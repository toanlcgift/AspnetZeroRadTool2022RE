using AspNetZeroRadToolVisualStudioExtension.Dialogs.Providers;
using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using AspNetZeroRadToolVisualStudioExtension.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs
{
  public class ForeignKeySelectionForm : Form
  {
    private readonly List<NavigationProperty> _entityNavigationProperties;
    public NavigationProperty SelectedEntity;
    public NavigationProperty EditEntity;
    private IContainer components;
    private Button btnCancel;
    private Button btnOk;
    private Button button1;
    private ComboBox cbEntitySelector;
    private CheckBox cbNullable;
    private ComboBox cmbDisplayProperty;
    private ComboBox cmbRelationType;
    private ComboBox cmbViewType;
    private Label label1;
    private Label label2;
    private Label label3;
    private Label label4;
    private Label label5;
    private Panel pnlBottom;
    private TextBox txtPropertyName;

    public ForeignKeySelectionForm(
      NavigationProperty editEntity,
      List<NavigationProperty> entityNavigationProperties)
    {
      this._entityNavigationProperties = entityNavigationProperties;
      if (editEntity != null)
        this.EditEntity = editEntity;
      this.InitializeComponent();
    }

    private void ForeignKeySelectionForm_Load(object sender, EventArgs e) => this.PopulateEntityCombobox(true);

    public void PopulateEntityCombobox(bool clearListBeforeInsert, string directory = null)
    {
      this.cbEntitySelector.Enabled = false;
      this.cmbRelationType.Enabled = false;
      try
      {
        EntityProvider.StoreAllEntitiesDefinitionsInSolution(clearListBeforeInsert, directory);
        List<NavigationProperty> entityList = EntityProvider.EntityList;
        this.cbEntitySelector.Items.Clear();
        int num = 0;
        foreach (NavigationProperty navigationProperty in entityList)
        {
          this.cbEntitySelector.Items.Add((object) new ComboboxItem()
          {
            Text = (navigationProperty.ForeignEntityName + " {" + navigationProperty.Namespace + "} " + navigationProperty.IdType),
            Value = (object) navigationProperty.ForeignEntityName
          });
          if (this.EditEntity != null && navigationProperty.ForeignEntityName == this.EditEntity.ForeignEntityName && navigationProperty.Namespace == this.EditEntity.Namespace)
            this.cbEntitySelector.SelectedIndex = num;
          ++num;
        }
        this.cmbRelationType.Items.Clear();
        this.cmbRelationType.Items.Add((object) new ComboboxItem()
        {
          Text = "single",
          Value = (object) "single"
        });
        this.cmbRelationType.Items.Add((object) new ComboboxItem()
        {
          Text = "collection",
          Value = (object) "collection"
        });
        this.cmbRelationType.SelectedIndex = 0;
        this.cmbViewType.SelectedIndex = 0;
        if (this.EditEntity == null)
          return;
        this.txtPropertyName.Text = this.EditEntity.PropertyName;
        this.cmbDisplayProperty.Text = this.EditEntity.DisplayPropertyName;
        this.cbNullable.Checked = this.EditEntity.IsNullable;
        if (!(this.EditEntity.ViewType == "Dropdown"))
          return;
        this.cmbViewType.SelectedIndex = 1;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Unable to find entities in the solution", ex);
      }
      finally
      {
        this.cbEntitySelector.Enabled = true;
        this.cmbRelationType.Enabled = true;
      }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.SelectedEntity == null)
        {
          MsgBox.Warn("Select your entity!");
          this.cbEntitySelector.Focus();
        }
        else if (string.IsNullOrWhiteSpace(this.txtPropertyName.Text))
        {
          MsgBox.Warn("Enter a property name");
          this.txtPropertyName.Focus();
        }
        else if (string.IsNullOrWhiteSpace(this.SelectedEntity.DisplayPropertyName))
        {
          MsgBox.Warn("Select the display property!");
          this.cmbDisplayProperty.Focus();
        }
        else
        {
          this.SelectedEntity.PropertyName = this.txtPropertyName.Text;
          this.SelectedEntity.RelationType = this.cmbRelationType.Text;
          this.SelectedEntity.IsNullable = this.cbNullable.Checked;
          this.SelectedEntity.ViewType = this.cmbViewType.SelectedItem.ToString();
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while selecting foreign key!", ex);
      }
    }

    private void cbEntitySelector_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        this.SelectedEntity = EntityProvider.EntityList[this.cbEntitySelector.SelectedIndex];
        this.txtPropertyName.Text = string.Format("{0}Id", (object) this.SelectedEntity.ForeignEntityName);
        this.cmbDisplayProperty.Items.Clear();
        foreach (string displayName in this.SelectedEntity.DisplayNameList)
          this.cmbDisplayProperty.Items.Add((object) new ComboboxItem()
          {
            Text = displayName,
            Value = (object) displayName
          });
        if (this.SelectedEntity.DisplayNameList.Any<string>((Func<string, bool>) (dn => dn.ToLower().Contains("name"))))
          this.cmbDisplayProperty.SelectedIndex = this.SelectedEntity.DisplayNameList.FindIndex((Predicate<string>) (dn => dn.ToLower().Contains("name")));
        else
          this.cmbDisplayProperty.SelectedIndex = 0;
        if (this.EditEntity == null)
          return;
        this.cmbDisplayProperty.SelectedIndex = this.SelectedEntity.DisplayNameList.FindIndex((Predicate<string>) (dn => dn.ToLower().Contains(this.EditEntity.DisplayPropertyName.ToLower())));
      }
      catch (Exception ex)
      {
        ExceptionHandler.Log(ex, "ForeignKeySelectionForm > cbEntitySelector_SelectedIndexChanged throws exception");
      }
    }

    private void cmbDisplayProperty_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        if (this.SelectedEntity == null)
          return;
        this.SelectedEntity.DisplayPropertyName = this.cmbDisplayProperty.Text;
      }
      catch (Exception ex)
      {
        ExceptionHandler.Log(ex, "ForeignKeySelectionForm > cmbDisplayProperty_SelectedIndexChanged throws exception");
      }
    }

    private void btnCancel_Click(object sender, EventArgs e) => this.Close();

    private void button1_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      folderBrowserDialog.SelectedPath = EntityProvider._slnSrcCoreDirectory;
      if (folderBrowserDialog.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
        return;
      this.PopulateEntityCombobox(false, folderBrowserDialog.SelectedPath);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ForeignKeySelectionForm));
      this.label1 = new Label();
      this.btnOk = new Button();
      this.cbEntitySelector = new ComboBox();
      this.cmbDisplayProperty = new ComboBox();
      this.txtPropertyName = new TextBox();
      this.label2 = new Label();
      this.label3 = new Label();
      this.cmbRelationType = new ComboBox();
      this.label4 = new Label();
      this.cbNullable = new CheckBox();
      this.pnlBottom = new Panel();
      this.btnCancel = new Button();
      this.button1 = new Button();
      this.label5 = new Label();
      this.cmbViewType = new ComboBox();
      this.pnlBottom.SuspendLayout();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Segoe UI", 9.75f);
      this.label1.ForeColor = Color.FromArgb(221, 221, 221);
      this.label1.Location = new Point(12, 23);
      this.label1.Name = "label1";
      this.label1.Size = new Size(107, 17);
      this.label1.TabIndex = 5;
      this.label1.Text = "Select Your Entity";
      this.btnOk.BackColor = Color.FromArgb(3, 184, 120);
      this.btnOk.Cursor = Cursors.Hand;
      this.btnOk.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnOk.FlatStyle = FlatStyle.Flat;
      this.btnOk.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnOk.ForeColor = Color.White;
      this.btnOk.Location = new Point(371, 12);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new Size(125, 27);
      this.btnOk.TabIndex = 6;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = false;
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      this.cbEntitySelector.BackColor = Color.FromArgb(224, 224, 224);
      this.cbEntitySelector.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbEntitySelector.FlatStyle = FlatStyle.Flat;
      this.cbEntitySelector.Font = new Font("Segoe UI", 9.75f);
      this.cbEntitySelector.FormattingEnabled = true;
      this.cbEntitySelector.Location = new Point(12, 46);
      this.cbEntitySelector.Name = "cbEntitySelector";
      this.cbEntitySelector.Size = new Size(484, 25);
      this.cbEntitySelector.TabIndex = 1;
      this.cbEntitySelector.SelectedIndexChanged += new EventHandler(this.cbEntitySelector_SelectedIndexChanged);
      this.cmbDisplayProperty.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbDisplayProperty.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbDisplayProperty.FlatStyle = FlatStyle.Flat;
      this.cmbDisplayProperty.Font = new Font("Segoe UI", 9.75f);
      this.cmbDisplayProperty.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbDisplayProperty.FormattingEnabled = true;
      this.cmbDisplayProperty.Location = new Point(125, 149);
      this.cmbDisplayProperty.Name = "cmbDisplayProperty";
      this.cmbDisplayProperty.Size = new Size(151, 25);
      this.cmbDisplayProperty.TabIndex = 3;
      this.cmbDisplayProperty.SelectedIndexChanged += new EventHandler(this.cmbDisplayProperty_SelectedIndexChanged);
      this.txtPropertyName.BackColor = Color.FromArgb(224, 224, 224);
      this.txtPropertyName.BorderStyle = BorderStyle.FixedSingle;
      this.txtPropertyName.Font = new Font("Segoe UI", 9.75f);
      this.txtPropertyName.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtPropertyName.Location = new Point(125, 101);
      this.txtPropertyName.Name = "txtPropertyName";
      this.txtPropertyName.Size = new Size(371, 25);
      this.txtPropertyName.TabIndex = 2;
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Segoe UI", 9.75f);
      this.label2.ForeColor = Color.FromArgb(221, 221, 221);
      this.label2.Location = new Point(9, 103);
      this.label2.Name = "label2";
      this.label2.Size = new Size(97, 17);
      this.label2.TabIndex = 8;
      this.label2.Text = "Property Name";
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Segoe UI", 9.75f);
      this.label3.ForeColor = Color.FromArgb(221, 221, 221);
      this.label3.Location = new Point(9, 153);
      this.label3.Name = "label3";
      this.label3.Size = new Size(104, 17);
      this.label3.TabIndex = 9;
      this.label3.Text = "Display Property";
      this.cmbRelationType.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbRelationType.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbRelationType.FlatStyle = FlatStyle.Flat;
      this.cmbRelationType.Font = new Font("Segoe UI", 9.75f);
      this.cmbRelationType.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbRelationType.FormattingEnabled = true;
      this.cmbRelationType.Location = new Point(405, 193);
      this.cmbRelationType.Name = "cmbRelationType";
      this.cmbRelationType.Size = new Size(91, 25);
      this.cmbRelationType.TabIndex = 4;
      this.cmbRelationType.Visible = false;
      this.label4.AutoSize = true;
      this.label4.Font = new Font("Segoe UI", 9.75f);
      this.label4.ForeColor = Color.FromArgb(221, 221, 221);
      this.label4.Location = new Point(313, 196);
      this.label4.Name = "label4";
      this.label4.Size = new Size(86, 17);
      this.label4.TabIndex = 11;
      this.label4.Text = "Relation Type";
      this.label4.Visible = false;
      this.cbNullable.AutoSize = true;
      this.cbNullable.Checked = true;
      this.cbNullable.CheckState = CheckState.Checked;
      this.cbNullable.Cursor = Cursors.Hand;
      this.cbNullable.Font = new Font("Segoe UI", 9.75f);
      this.cbNullable.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbNullable.Location = new Point(-1, 195);
      this.cbNullable.Name = "cbNullable";
      this.cbNullable.RightToLeft = RightToLeft.Yes;
      this.cbNullable.Size = new Size(139, 21);
      this.cbNullable.TabIndex = 5;
      this.cbNullable.Text = "   Nullable             ";
      this.cbNullable.UseVisualStyleBackColor = true;
      this.pnlBottom.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlBottom.Controls.Add((Control) this.btnCancel);
      this.pnlBottom.Controls.Add((Control) this.btnOk);
      this.pnlBottom.Dock = DockStyle.Bottom;
      this.pnlBottom.Location = new Point(0, 238);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new Size(508, 51);
      this.pnlBottom.TabIndex = 13;
      this.btnCancel.BackColor = Color.Gray;
      this.btnCancel.Cursor = Cursors.Hand;
      this.btnCancel.DialogResult = DialogResult.Cancel;
      this.btnCancel.FlatAppearance.BorderColor = Color.Gray;
      this.btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatStyle = FlatStyle.Flat;
      this.btnCancel.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnCancel.ForeColor = Color.White;
      this.btnCancel.Location = new Point(284, 12);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new Size(64, 27);
      this.btnCancel.TabIndex = 7;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = false;
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      this.button1.BackColor = Color.FromArgb(3, 184, 120);
      this.button1.Cursor = Cursors.Hand;
      this.button1.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.button1.FlatStyle = FlatStyle.Flat;
      this.button1.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
      this.button1.ForeColor = Color.White;
      this.button1.Location = new Point(414, 12);
      this.button1.Name = "button1";
      this.button1.Size = new Size(82, 28);
      this.button1.TabIndex = 14;
      this.button1.Text = "Load More";
      this.button1.UseVisualStyleBackColor = false;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Segoe UI", 9.75f);
      this.label5.ForeColor = Color.FromArgb(221, 221, 221);
      this.label5.Location = new Point(295, 153);
      this.label5.Name = "label5";
      this.label5.Size = new Size(104, 17);
      this.label5.TabIndex = 16;
      this.label5.Text = "Select View Type";
      this.cmbViewType.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbViewType.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbViewType.FlatStyle = FlatStyle.Flat;
      this.cmbViewType.Font = new Font("Segoe UI", 9.75f);
      this.cmbViewType.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbViewType.FormattingEnabled = true;
      this.cmbViewType.Items.AddRange(new object[2]
      {
        (object) "LookupTable",
        (object) "Dropdown"
      });
      this.cmbViewType.Location = new Point(405, 149);
      this.cmbViewType.Name = "cmbViewType";
      this.cmbViewType.Size = new Size(91, 25);
      this.cmbViewType.TabIndex = 15;
      this.AcceptButton = (IButtonControl) this.btnOk;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(38, 38, 42);
      this.CancelButton = (IButtonControl) this.btnCancel;
      this.ClientSize = new Size(508, 289);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.cmbViewType);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.pnlBottom);
      this.Controls.Add((Control) this.cbNullable);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.cmbRelationType);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.txtPropertyName);
      this.Controls.Add((Control) this.cmbDisplayProperty);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.cbEntitySelector);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (ForeignKeySelectionForm);
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Foreign Key";
      this.Load += new EventHandler(this.ForeignKeySelectionForm_Load);
      this.pnlBottom.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
