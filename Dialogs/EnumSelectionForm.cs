using AspNetZeroRadToolVisualStudioExtension.Dialogs.Providers;
using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using AspNetZeroRadToolVisualStudioExtension.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs
{
  public class EnumSelectionForm : Form
  {
    public EnumDefinition SelectedEnum;
    private string _emptyEnumLabelText = "-";
    private IContainer components;
    private Button btnCancel;
    private Button btnOk;
    private ComboBox cmbEnum;
    private Label label1;
    private Label lblSelectedEnumName;
    private Label lblSelectedEnumNameCaption;
    private Label lblSelectedEnumNamespace;
    private Label lblSelectedEnumNamespaceCaption;
    private Panel pnlBottom;

    public EnumSelectionForm() => this.InitializeComponent();

    private void EnumSelectionForm_Load(object sender, EventArgs e)
    {
      try
      {
        this.lblSelectedEnumName.Text = this.lblSelectedEnumNamespace.Text = this._emptyEnumLabelText;
        this.PopulateEnumList();
        this.cmbEnum.Focus();
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Unable to find your enums in the solution!", ex);
      }
    }

    public void PopulateEnumList()
    {
      try
      {
        this.cmbEnum.Enabled = false;
        EnumProvider.StoreAllEnumDefinitionsInSolution();
        foreach (EnumDefinition enumDefinition in EnumProvider.EnumList)
          this.cmbEnum.Items.Add((object) new ComboboxItem()
          {
            Text = (enumDefinition.Name + " {" + enumDefinition.Namespace + "}"),
            Value = (object) enumDefinition.Name
          });
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error while populating enum list!", ex);
      }
      finally
      {
        this.cmbEnum.Enabled = true;
      }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.SelectedEnum == null)
        {
          MsgBox.Exception("Select an enum from the list!");
        }
        else
        {
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while selecting enum!", ex);
      }
    }

    private bool SelectEnum(out string enumName, out string enumNamespace)
    {
      enumName = (string) null;
      enumNamespace = (string) null;
      try
      {
        string[] strArray = this.cmbEnum.Text.Split(' ');
        enumName = strArray[0];
        enumNamespace = strArray[strArray.Length - 1];
        enumNamespace = enumNamespace.Substring(1, enumNamespace.Length - 2);
        string nameOfEnum = enumName;
        string namespaceOfEnum = enumNamespace;
        this.SelectedEnum = EnumProvider.EnumList.Find((Predicate<EnumDefinition>) (ed => ed.Name == nameOfEnum && ed.Namespace == namespaceOfEnum));
        if (this.SelectedEnum == null)
        {
          MsgBox.Exception("Cannot find the enum: \"" + enumName + "\"");
          return false;
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Cannot retrieve enum name and enum namespace!", ex);
        return false;
      }
      return true;
    }

    private void cmbEnum_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        this.cmbEnum.Enabled = false;
        string enumName;
        string enumNamespace;
        if (this.SelectEnum(out enumName, out enumNamespace))
        {
          this.lblSelectedEnumName.Text = enumName;
          this.lblSelectedEnumNamespace.Text = enumNamespace;
        }
        else
          this.lblSelectedEnumNamespace.Text = this.lblSelectedEnumName.Text = this._emptyEnumLabelText;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error while selecting the enum!", ex);
      }
      finally
      {
        this.cmbEnum.Enabled = true;
      }
    }

    private void btnCancel_Click(object sender, EventArgs e) => this.Close();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (EnumSelectionForm));
      this.cmbEnum = new ComboBox();
      this.btnOk = new Button();
      this.label1 = new Label();
      this.pnlBottom = new Panel();
      this.btnCancel = new Button();
      this.lblSelectedEnumNameCaption = new Label();
      this.lblSelectedEnumNamespaceCaption = new Label();
      this.lblSelectedEnumName = new Label();
      this.lblSelectedEnumNamespace = new Label();
      this.pnlBottom.SuspendLayout();
      this.SuspendLayout();
      this.cmbEnum.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbEnum.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbEnum.Enabled = false;
      this.cmbEnum.FlatStyle = FlatStyle.Flat;
      this.cmbEnum.Font = new Font("Segoe UI", 9.75f);
      this.cmbEnum.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbEnum.FormattingEnabled = true;
      this.cmbEnum.Location = new Point(12, 32);
      this.cmbEnum.Name = "cmbEnum";
      this.cmbEnum.Size = new Size(473, 25);
      this.cmbEnum.TabIndex = 1;
      this.cmbEnum.SelectedIndexChanged += new EventHandler(this.cmbEnum_SelectedIndexChanged);
      this.btnOk.BackColor = Color.FromArgb(3, 184, 120);
      this.btnOk.Cursor = Cursors.Hand;
      this.btnOk.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnOk.FlatStyle = FlatStyle.Flat;
      this.btnOk.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnOk.ForeColor = Color.White;
      this.btnOk.Location = new Point(360, 23);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new Size(125, 27);
      this.btnOk.TabIndex = 2;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = false;
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Segoe UI", 9.75f);
      this.label1.ForeColor = Color.FromArgb(221, 221, 221);
      this.label1.Location = new Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new Size(108, 17);
      this.label1.TabIndex = 2;
      this.label1.Text = "Select Your Enum";
      this.pnlBottom.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlBottom.Controls.Add((Control) this.btnCancel);
      this.pnlBottom.Controls.Add((Control) this.btnOk);
      this.pnlBottom.Dock = DockStyle.Bottom;
      this.pnlBottom.Location = new Point(0, 137);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new Size(499, 62);
      this.pnlBottom.TabIndex = 14;
      this.btnCancel.BackColor = Color.Gray;
      this.btnCancel.Cursor = Cursors.Hand;
      this.btnCancel.DialogResult = DialogResult.Cancel;
      this.btnCancel.FlatAppearance.BorderColor = Color.Gray;
      this.btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatStyle = FlatStyle.Flat;
      this.btnCancel.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnCancel.ForeColor = Color.White;
      this.btnCancel.Location = new Point(277, 23);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new Size(64, 27);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = false;
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      this.lblSelectedEnumNameCaption.AutoSize = true;
      this.lblSelectedEnumNameCaption.Font = new Font("Segoe UI", 9.75f);
      this.lblSelectedEnumNameCaption.ForeColor = Color.FromArgb(221, 221, 221);
      this.lblSelectedEnumNameCaption.Location = new Point(12, 74);
      this.lblSelectedEnumNameCaption.Name = "lblSelectedEnumNameCaption";
      this.lblSelectedEnumNameCaption.Size = new Size(82, 17);
      this.lblSelectedEnumNameCaption.TabIndex = 15;
      this.lblSelectedEnumNameCaption.Text = "Enum Name:";
      this.lblSelectedEnumNamespaceCaption.AutoSize = true;
      this.lblSelectedEnumNamespaceCaption.Font = new Font("Segoe UI", 9.75f);
      this.lblSelectedEnumNamespaceCaption.ForeColor = Color.FromArgb(221, 221, 221);
      this.lblSelectedEnumNamespaceCaption.Location = new Point(12, 105);
      this.lblSelectedEnumNamespaceCaption.Name = "lblSelectedEnumNamespaceCaption";
      this.lblSelectedEnumNamespaceCaption.Size = new Size(116, 17);
      this.lblSelectedEnumNamespaceCaption.TabIndex = 16;
      this.lblSelectedEnumNamespaceCaption.Text = "Enum Namespace:";
      this.lblSelectedEnumName.AutoSize = true;
      this.lblSelectedEnumName.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
      this.lblSelectedEnumName.ForeColor = Color.FromArgb(221, 221, 221);
      this.lblSelectedEnumName.Location = new Point(94, 74);
      this.lblSelectedEnumName.Name = "lblSelectedEnumName";
      this.lblSelectedEnumName.Size = new Size(13, 17);
      this.lblSelectedEnumName.TabIndex = 17;
      this.lblSelectedEnumName.Text = "-";
      this.lblSelectedEnumNamespace.AutoSize = true;
      this.lblSelectedEnumNamespace.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
      this.lblSelectedEnumNamespace.ForeColor = Color.FromArgb(221, 221, 221);
      this.lblSelectedEnumNamespace.Location = new Point((int) sbyte.MaxValue, 105);
      this.lblSelectedEnumNamespace.Name = "lblSelectedEnumNamespace";
      this.lblSelectedEnumNamespace.Size = new Size(13, 17);
      this.lblSelectedEnumNamespace.TabIndex = 18;
      this.lblSelectedEnumNamespace.Text = "-";
      this.AcceptButton = (IButtonControl) this.btnOk;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(38, 38, 42);
      this.CancelButton = (IButtonControl) this.btnCancel;
      this.ClientSize = new Size(499, 199);
      this.Controls.Add((Control) this.lblSelectedEnumNamespace);
      this.Controls.Add((Control) this.lblSelectedEnumName);
      this.Controls.Add((Control) this.lblSelectedEnumNamespaceCaption);
      this.Controls.Add((Control) this.lblSelectedEnumNameCaption);
      this.Controls.Add((Control) this.pnlBottom);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.cmbEnum);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (EnumSelectionForm);
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Enum";
      this.Load += new EventHandler(this.EnumSelectionForm_Load);
      this.pnlBottom.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
