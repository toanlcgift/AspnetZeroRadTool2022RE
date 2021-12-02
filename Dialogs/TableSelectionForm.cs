using AspNetZeroRadToolVisualStudioExtension.Dialogs.Providers;
using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using AspNetZeroRadToolVisualStudioExtension.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs
{
  public class TableSelectionForm : Form
  {
    private readonly string _entityName;
    private readonly Entity _entity;
    private NavigationPropertyOneToManyTable _editTable;
    public NavigationPropertyOneToManyTable SelectedTable;
    private IContainer components;
    private Button btnCancel;
    private Button btnOk;
    private ComboBox cbEntitySelector;
    private Label label1;
    private Panel pnlBottom;
    private Label label2;
    private TextBox txtPropertyName;
    private Label label5;
    private ComboBox cmbViewType;
    private CheckBox cbNullable;
    private Label label3;
    private ComboBox cmbDisplayProperty;

    public TableSelectionForm(
      string entityName,
      Entity entity,
      NavigationPropertyOneToManyTable editTable = null)
    {
      this._entityName = entityName;
      this._entity = entity;
      this._editTable = editTable;
      this.InitializeComponent();
    }

    private void TableSelectionForm_Load(object sender, EventArgs e)
    {
      this.PopulateEntityCombobox(EntityProvider._slnDirectory);
      this.PopulateDisplayPropertyCombobox();
      this.cmbViewType.SelectedIndex = 0;
      this.cbEntitySelector.Enabled = true;
      if (this._editTable == null)
      {
        this.txtPropertyName.Text = this._entityName + "Id";
      }
      else
      {
        if (this._editTable.ViewType == "Dropdown")
          this.cmbViewType.SelectedIndex = 1;
        this.cbNullable.Checked = this._editTable.IsNullable;
        this.txtPropertyName.Text = this._editTable.ForeignPropertyName;
      }
    }

    private void PopulateDisplayPropertyCombobox()
    {
      int num = 0;
      foreach (Property property in this._entity.Properties)
      {
        this.cmbDisplayProperty.Items.Add((object) new ComboboxItem()
        {
          Text = property.Name,
          Value = (object) property.Name
        });
        if (this._editTable != null && this._editTable.DisplayPropertyName == property.Name)
          this.cmbDisplayProperty.SelectedIndex = num;
        ++num;
      }
    }

    private void cbEntitySelector_SelectedIndexChanged(object sender, EventArgs e)
    {
      ComboboxItem selectedItem = (ComboboxItem) this.cbEntitySelector.SelectedItem;
      if (selectedItem == null)
        MsgBox.Warn("Select your entity!");
      else
        this.SelectedTable = new NavigationPropertyOneToManyTable()
        {
          EntityJson = selectedItem.Text + ".json"
        };
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.SelectedTable == null)
        {
          MsgBox.Warn("Select a table!");
          this.cbEntitySelector.Focus();
        }
        else if (string.IsNullOrEmpty(this.txtPropertyName.Text))
        {
          MsgBox.Warn("Enter a property name");
          this.txtPropertyName.Focus();
        }
        else
        {
          this.SelectedTable.ForeignPropertyName = this.txtPropertyName.Text;
          this.SelectedTable.DisplayPropertyName = this.cmbDisplayProperty.Text;
          this.SelectedTable.IsNullable = this.cbNullable.Checked;
          this.SelectedTable.ViewType = this.cmbViewType.Text;
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while selecting table!", ex);
      }
    }

    private void PopulateEntityCombobox(string path)
    {
      IEnumerable<string> jsonFiles = TableSelectionForm.GetJsonFiles(path);
      int num = 0;
      foreach (string str in jsonFiles)
      {
        this.cbEntitySelector.Items.Add((object) new ComboboxItem()
        {
          Text = str,
          Value = (object) str
        });
        if (this._editTable != null && str == Path.GetFileNameWithoutExtension(this._editTable.EntityJson))
          this.cbEntitySelector.SelectedIndex = num;
        ++num;
      }
    }

    private static IEnumerable<string> GetJsonFiles(string path) => ((IEnumerable<FileInfo>) new DirectoryInfo(Path.Combine(path, AppSettings.PowerToolsDirectoryName)).GetFiles("*.json")).Where<FileInfo>((Func<FileInfo, bool>) (f => !f.Name.Equals("config.json", StringComparison.OrdinalIgnoreCase) && !f.Name.StartsWith("AspNetZeroRadTool.", StringComparison.OrdinalIgnoreCase))).Select<FileInfo, string>((Func<FileInfo, string>) (f => Path.GetFileNameWithoutExtension(f.Name)));

    private void btnCancel_Click(object sender, EventArgs e) => this.Close();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (TableSelectionForm));
      this.label1 = new Label();
      this.btnOk = new Button();
      this.pnlBottom = new Panel();
      this.btnCancel = new Button();
      this.cbEntitySelector = new ComboBox();
      this.label2 = new Label();
      this.txtPropertyName = new TextBox();
      this.label5 = new Label();
      this.cmbViewType = new ComboBox();
      this.cbNullable = new CheckBox();
      this.label3 = new Label();
      this.cmbDisplayProperty = new ComboBox();
      this.pnlBottom.SuspendLayout();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Segoe UI", 9.75f);
      this.label1.ForeColor = Color.FromArgb(221, 221, 221);
      this.label1.Location = new Point(13, 17);
      this.label1.Name = "label1";
      this.label1.Size = new Size(107, 17);
      this.label1.TabIndex = 21;
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
      this.pnlBottom.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlBottom.Controls.Add((Control) this.btnCancel);
      this.pnlBottom.Controls.Add((Control) this.btnOk);
      this.pnlBottom.Dock = DockStyle.Bottom;
      this.pnlBottom.Location = new Point(0, 212);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new Size(514, 51);
      this.pnlBottom.TabIndex = 26;
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
      this.cbEntitySelector.BackColor = Color.FromArgb(224, 224, 224);
      this.cbEntitySelector.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbEntitySelector.FlatStyle = FlatStyle.Flat;
      this.cbEntitySelector.Font = new Font("Segoe UI", 9.75f);
      this.cbEntitySelector.FormattingEnabled = true;
      this.cbEntitySelector.Location = new Point(13, 40);
      this.cbEntitySelector.Name = "cbEntitySelector";
      this.cbEntitySelector.Size = new Size(484, 25);
      this.cbEntitySelector.TabIndex = 17;
      this.cbEntitySelector.SelectedIndexChanged += new EventHandler(this.cbEntitySelector_SelectedIndexChanged);
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Segoe UI", 9.75f);
      this.label2.ForeColor = Color.FromArgb(221, 221, 221);
      this.label2.Location = new Point(10, 83);
      this.label2.Name = "label2";
      this.label2.Size = new Size(97, 17);
      this.label2.TabIndex = 28;
      this.label2.Text = "Property Name";
      this.txtPropertyName.BackColor = Color.FromArgb(224, 224, 224);
      this.txtPropertyName.BorderStyle = BorderStyle.FixedSingle;
      this.txtPropertyName.Font = new Font("Segoe UI", 9.75f);
      this.txtPropertyName.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtPropertyName.Location = new Point(126, 81);
      this.txtPropertyName.Name = "txtPropertyName";
      this.txtPropertyName.Size = new Size(371, 25);
      this.txtPropertyName.TabIndex = 27;
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Segoe UI", 9.75f);
      this.label5.ForeColor = Color.FromArgb(221, 221, 221);
      this.label5.Location = new Point(296, 126);
      this.label5.Name = "label5";
      this.label5.Size = new Size(104, 17);
      this.label5.TabIndex = 21;
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
      this.cmbViewType.Location = new Point(406, 122);
      this.cmbViewType.Name = "cmbViewType";
      this.cmbViewType.Size = new Size(91, 25);
      this.cmbViewType.TabIndex = 20;
      this.cbNullable.AutoSize = true;
      this.cbNullable.Checked = true;
      this.cbNullable.CheckState = CheckState.Checked;
      this.cbNullable.Cursor = Cursors.Hand;
      this.cbNullable.Font = new Font("Segoe UI", 9.75f);
      this.cbNullable.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbNullable.Location = new Point(0, 168);
      this.cbNullable.Name = "cbNullable";
      this.cbNullable.RightToLeft = RightToLeft.Yes;
      this.cbNullable.Size = new Size(139, 21);
      this.cbNullable.TabIndex = 18;
      this.cbNullable.Text = "   Nullable             ";
      this.cbNullable.UseVisualStyleBackColor = true;
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Segoe UI", 9.75f);
      this.label3.ForeColor = Color.FromArgb(221, 221, 221);
      this.label3.Location = new Point(10, 126);
      this.label3.Name = "label3";
      this.label3.Size = new Size(104, 17);
      this.label3.TabIndex = 19;
      this.label3.Text = "Display Property";
      this.cmbDisplayProperty.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbDisplayProperty.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbDisplayProperty.FlatStyle = FlatStyle.Flat;
      this.cmbDisplayProperty.Font = new Font("Segoe UI", 9.75f);
      this.cmbDisplayProperty.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbDisplayProperty.FormattingEnabled = true;
      this.cmbDisplayProperty.Location = new Point(126, 122);
      this.cmbDisplayProperty.Name = "cmbDisplayProperty";
      this.cmbDisplayProperty.Size = new Size(151, 25);
      this.cmbDisplayProperty.TabIndex = 17;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(38, 38, 42);
      this.ClientSize = new Size(514, 263);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.cmbViewType);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.cbNullable);
      this.Controls.Add((Control) this.txtPropertyName);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.cmbDisplayProperty);
      this.Controls.Add((Control) this.pnlBottom);
      this.Controls.Add((Control) this.cbEntitySelector);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Name = nameof (TableSelectionForm);
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Table Selection Form";
      this.Load += new EventHandler(this.TableSelectionForm_Load);
      this.pnlBottom.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
