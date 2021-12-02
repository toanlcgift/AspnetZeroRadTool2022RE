using AspNetZeroRadToolVisualStudioExtension.Dialogs.EntityLoaders.Database;
using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using AspNetZeroRadToolVisualStudioExtension.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs
{
  public class DbTableSelectionForm : Form
  {
    public Entity SelectedTable;
    public readonly string SolutionDirectory;
    public readonly Entity BaseEntity;
    public List<Entity> EntityList;
    private string _defaultConnectionString;
    private IContainer components;
    private Button btnOk;
    private ComboBox cmbTable;
    private Button btnRetrieveTables;
    private Label label1;
    private Label label2;
    private Label label3;
    private Panel pnlBottom;
    private TextBox txtConnectionString;
    private Button btnCancel;
    private Button btnTestConnection;
    private RadioButton rbSqlServer;
    private RadioButton rbMySql;
    private RadioButton rbPostgreSql;

    public DbTableSelectionForm(string solutionDirectory, Entity baseEntity)
    {
      this.InitializeComponent();
      this.SolutionDirectory = solutionDirectory;
      this.BaseEntity = baseEntity;
    }

    private void DbTableSelectionForm_Load(object sender, EventArgs e)
    {
      try
      {
        this._defaultConnectionString = DbTableSelectionForm.CreateConnectionString(this.SolutionDirectory);
        this.txtConnectionString.Focus();
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while opening table selection form!", ex);
      }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      try
      {
        this.btnOk.Enabled = false;
        ComboboxItem selectedTable = (ComboboxItem) this.cmbTable.SelectedItem;
        if (selectedTable == null)
        {
          MsgBox.Warn("No table selected!");
        }
        else
        {
          this.SelectedTable = this.EntityList.FirstOrDefault<Entity>((Func<Entity, bool>) (en => en.EntityName.Equals(selectedTable.Text)));
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while selecting entity!", ex);
      }
      finally
      {
        this.btnOk.Enabled = true;
      }
    }

    private void LoadEntitiesFromDatabase(string connectionString)
    {
      try
      {
        EntityLoaderFromDatabase loaderFromDatabase = this.GetEntityLoaderFromDatabase();
        if (loaderFromDatabase == null)
          return;
        this.EntityList = loaderFromDatabase.GetTablesAsEntity(connectionString, this.BaseEntity);
        if (this.EntityList.Count <= 0)
        {
          MsgBox.Warn("There is no table in the database!");
        }
        else
        {
          this.cmbTable.Items.Clear();
          this.EntityList = this.EntityList.OrderBy<Entity, string>((Func<Entity, string>) (e => e.EntityName)).ToList<Entity>();
          foreach (Entity entity in this.EntityList)
            this.cmbTable.Items.Add((object) new ComboboxItem()
            {
              Text = entity.EntityName,
              Value = (object) entity.EntityName
            });
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Cannot load entities from database!", ex);
      }
    }

    private static string CreateConnectionString(string solutionDirectory)
    {
      try
      {
        string path = Path.Combine(solutionDirectory, "src");
        using (StreamReader streamReader = File.OpenText(Path.Combine(((IEnumerable<string>) Directory.GetDirectories(path)).Any<string>((Func<string, bool>) (d => d.ToLower().EndsWith(".mvc"))) ? ((IEnumerable<string>) Directory.GetDirectories(path)).First<string>((Func<string, bool>) (d => d.ToLower().EndsWith(".mvc"))) : ((IEnumerable<string>) Directory.GetDirectories(path)).First<string>((Func<string, bool>) (d => d.ToLower().EndsWith(".host"))), "appsettings.json")))
        {
          using (JsonTextReader jsonTextReader = new JsonTextReader((TextReader) streamReader))
            return (string) ((JObject) JToken.ReadFrom((JsonReader) jsonTextReader))["ConnectionStrings"][(object) "Default"] ?? "";
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Log(ex);
        return "NotAvailable";
      }
    }

    private void btnRetrieveTables_Click(object sender, EventArgs e)
    {
      try
      {
        this.btnRetrieveTables.Enabled = false;
        this.cmbTable.Enabled = false;
        if (string.IsNullOrWhiteSpace(this.txtConnectionString.Text.Trim()))
          MsgBox.Warn("Enter your connection string!");
        else if (this._defaultConnectionString.Equals(this.txtConnectionString.Text))
          MsgBox.Warn("Cannot work with the same database of your project!\n\rConnect to a different database which this project is linked.");
        else
          this.LoadEntitiesFromDatabase(this.txtConnectionString.Text);
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while loading entities from database!", ex);
      }
      finally
      {
        this.btnRetrieveTables.Enabled = true;
        this.cmbTable.Enabled = this.cmbTable.Items.Count > 0;
        if (this.cmbTable.Enabled)
          this.cmbTable.DroppedDown = true;
      }
    }

    private void btnTestConnection_Click(object sender, EventArgs e)
    {
      try
      {
        this.btnTestConnection.Enabled = false;
        if (string.IsNullOrWhiteSpace(this.txtConnectionString.Text.Trim()))
        {
          MsgBox.Warn("Enter your connection string!");
        }
        else
        {
          EntityLoaderFromDatabase loaderFromDatabase = this.GetEntityLoaderFromDatabase();
          if (loaderFromDatabase == null)
            return;
          Exception exception;
          if (loaderFromDatabase.TestConnection(this.txtConnectionString.Text, out exception))
            MsgBox.Info("Test connection succeeded.");
          else
            MsgBox.Exception("Connection failed!", exception);
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while testing connection!", ex);
      }
      finally
      {
        this.btnTestConnection.Enabled = true;
      }
    }

    private void btnCancel_Click(object sender, EventArgs e) => this.Close();

    private void txtConnectionString_TextChanged(object sender, EventArgs e) => this.btnRetrieveTables.Enabled = !string.IsNullOrWhiteSpace(this.txtConnectionString.Text.Trim());

    private EntityLoaderFromDatabase GetEntityLoaderFromDatabase()
    {
      if (this.rbSqlServer.Checked)
        return (EntityLoaderFromDatabase) new EntityLoaderFromSqlDatabase();
      if (this.rbMySql.Checked)
        return (EntityLoaderFromDatabase) new EntityLoaderFromMySqlDatabase();
      if (this.rbPostgreSql.Checked)
        return (EntityLoaderFromDatabase) new EntityLoaderFromPostgreSqlDatabase();
      MsgBox.Exception("Unidentified data source!");
      return (EntityLoaderFromDatabase) null;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (DbTableSelectionForm));
      this.btnOk = new Button();
      this.cmbTable = new ComboBox();
      this.btnRetrieveTables = new Button();
      this.label1 = new Label();
      this.label2 = new Label();
      this.label3 = new Label();
      this.pnlBottom = new Panel();
      this.btnCancel = new Button();
      this.txtConnectionString = new TextBox();
      this.btnTestConnection = new Button();
      this.rbSqlServer = new RadioButton();
      this.rbMySql = new RadioButton();
      this.rbPostgreSql = new RadioButton();
      this.pnlBottom.SuspendLayout();
      this.SuspendLayout();
      this.btnOk.BackColor = Color.FromArgb(3, 184, 120);
      this.btnOk.Cursor = Cursors.Hand;
      this.btnOk.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnOk.FlatStyle = FlatStyle.Flat;
      this.btnOk.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnOk.ForeColor = Color.White;
      this.btnOk.Location = new Point(423, 21);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new Size(125, 27);
      this.btnOk.TabIndex = 7;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = false;
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      this.cmbTable.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbTable.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbTable.Enabled = false;
      this.cmbTable.FlatStyle = FlatStyle.Flat;
      this.cmbTable.Font = new Font("Segoe UI", 9.75f);
      this.cmbTable.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbTable.FormattingEnabled = true;
      this.cmbTable.Location = new Point(12, 166);
      this.cmbTable.Name = "cmbTable";
      this.cmbTable.Size = new Size(429, 25);
      this.cmbTable.TabIndex = 5;
      this.btnRetrieveTables.BackColor = Color.FromArgb(3, 184, 120);
      this.btnRetrieveTables.Cursor = Cursors.Hand;
      this.btnRetrieveTables.Enabled = false;
      this.btnRetrieveTables.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnRetrieveTables.FlatStyle = FlatStyle.Flat;
      this.btnRetrieveTables.Font = new Font("Segoe UI", 9f);
      this.btnRetrieveTables.ForeColor = Color.White;
      this.btnRetrieveTables.Location = new Point(447, 166);
      this.btnRetrieveTables.Name = "btnRetrieveTables";
      this.btnRetrieveTables.Size = new Size(104, 24);
      this.btnRetrieveTables.TabIndex = 6;
      this.btnRetrieveTables.Text = "Retrieve tables";
      this.btnRetrieveTables.UseVisualStyleBackColor = false;
      this.btnRetrieveTables.Click += new EventHandler(this.btnRetrieveTables_Click);
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Segoe UI", 11f);
      this.label1.ForeColor = Color.FromArgb(221, 221, 221);
      this.label1.Location = new Point(9, 69);
      this.label1.Name = "label1";
      this.label1.Size = new Size((int) sbyte.MaxValue, 20);
      this.label1.TabIndex = 5;
      this.label1.Text = "Connection String";
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Segoe UI", 11f);
      this.label2.ForeColor = Color.FromArgb(221, 221, 221);
      this.label2.Location = new Point(9, 143);
      this.label2.Name = "label2";
      this.label2.Size = new Size(44, 20);
      this.label2.TabIndex = 6;
      this.label2.Text = "Table";
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Segoe UI", 11f);
      this.label3.ForeColor = Color.FromArgb(221, 221, 221);
      this.label3.Location = new Point(9, 22);
      this.label3.Name = "label3";
      this.label3.Size = new Size(84, 20);
      this.label3.TabIndex = 8;
      this.label3.Text = "Datasource";
      this.pnlBottom.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlBottom.Controls.Add((Control) this.btnCancel);
      this.pnlBottom.Controls.Add((Control) this.btnOk);
      this.pnlBottom.Dock = DockStyle.Bottom;
      this.pnlBottom.Location = new Point(0, 223);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new Size(564, 60);
      this.pnlBottom.TabIndex = 9;
      this.btnCancel.BackColor = Color.Gray;
      this.btnCancel.Cursor = Cursors.Hand;
      this.btnCancel.DialogResult = DialogResult.Cancel;
      this.btnCancel.FlatAppearance.BorderColor = Color.Gray;
      this.btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatStyle = FlatStyle.Flat;
      this.btnCancel.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnCancel.ForeColor = Color.White;
      this.btnCancel.Location = new Point(333, 21);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new Size(64, 27);
      this.btnCancel.TabIndex = 8;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = false;
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      this.txtConnectionString.BackColor = Color.FromArgb(224, 224, 224);
      this.txtConnectionString.BorderStyle = BorderStyle.FixedSingle;
      this.txtConnectionString.Font = new Font("Segoe UI", 9.75f);
      this.txtConnectionString.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtConnectionString.Location = new Point(9, 92);
      this.txtConnectionString.Name = "txtConnectionString";
      this.txtConnectionString.RightToLeft = RightToLeft.No;
      this.txtConnectionString.Size = new Size(432, 25);
      this.txtConnectionString.TabIndex = 3;
      this.txtConnectionString.TextChanged += new EventHandler(this.txtConnectionString_TextChanged);
      this.btnTestConnection.BackColor = Color.FromArgb(3, 184, 120);
      this.btnTestConnection.Cursor = Cursors.Hand;
      this.btnTestConnection.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnTestConnection.FlatStyle = FlatStyle.Flat;
      this.btnTestConnection.Font = new Font("Segoe UI", 9f);
      this.btnTestConnection.ForeColor = Color.White;
      this.btnTestConnection.Location = new Point(447, 93);
      this.btnTestConnection.Name = "btnTestConnection";
      this.btnTestConnection.Size = new Size(104, 24);
      this.btnTestConnection.TabIndex = 4;
      this.btnTestConnection.Text = "Test connection";
      this.btnTestConnection.UseVisualStyleBackColor = false;
      this.btnTestConnection.Click += new EventHandler(this.btnTestConnection_Click);
      this.rbSqlServer.AutoSize = true;
      this.rbSqlServer.Checked = true;
      this.rbSqlServer.Cursor = Cursors.Hand;
      this.rbSqlServer.Font = new Font("Segoe UI", 9.75f);
      this.rbSqlServer.ForeColor = Color.White;
      this.rbSqlServer.Location = new Point(109, 22);
      this.rbSqlServer.Name = "rbSqlServer";
      this.rbSqlServer.Size = new Size(85, 21);
      this.rbSqlServer.TabIndex = 1;
      this.rbSqlServer.TabStop = true;
      this.rbSqlServer.Text = "Sql Server";
      this.rbSqlServer.UseVisualStyleBackColor = true;
      this.rbMySql.AutoSize = true;
      this.rbMySql.Cursor = Cursors.Hand;
      this.rbMySql.Font = new Font("Segoe UI", 9.75f);
      this.rbMySql.ForeColor = Color.White;
      this.rbMySql.Location = new Point(217, 22);
      this.rbMySql.Name = "rbMySql";
      this.rbMySql.Size = new Size(62, 21);
      this.rbMySql.TabIndex = 2;
      this.rbMySql.Text = "MySql";
      this.rbMySql.UseVisualStyleBackColor = true;
      this.rbPostgreSql.AutoSize = true;
      this.rbPostgreSql.Cursor = Cursors.Hand;
      this.rbPostgreSql.Font = new Font("Segoe UI", 9.75f);
      this.rbPostgreSql.ForeColor = Color.White;
      this.rbPostgreSql.Location = new Point(297, 22);
      this.rbPostgreSql.Name = "rbPostgreSql";
      this.rbPostgreSql.Size = new Size(89, 21);
      this.rbPostgreSql.TabIndex = 10;
      this.rbPostgreSql.Text = "PostgreSql";
      this.rbPostgreSql.UseVisualStyleBackColor = true;
      this.AcceptButton = (IButtonControl) this.btnOk;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(38, 38, 42);
      this.CancelButton = (IButtonControl) this.btnCancel;
      this.ClientSize = new Size(564, 283);
      this.Controls.Add((Control) this.rbPostgreSql);
      this.Controls.Add((Control) this.rbMySql);
      this.Controls.Add((Control) this.rbSqlServer);
      this.Controls.Add((Control) this.btnTestConnection);
      this.Controls.Add((Control) this.txtConnectionString);
      this.Controls.Add((Control) this.pnlBottom);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.btnRetrieveTables);
      this.Controls.Add((Control) this.cmbTable);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (DbTableSelectionForm);
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Database Table";
      this.Load += new EventHandler(this.DbTableSelectionForm_Load);
      this.pnlBottom.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
