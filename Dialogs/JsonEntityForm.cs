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
  public class JsonEntityForm : Form
  {
    private readonly string _solutionPath;
    public string SelectedFile = "";
    private IContainer components;
    private ComboBox cmbEntity;
    private Button btnOk;
    private Panel pnlBottom;
    private Label label1;
    private Button btnCancel;

    public JsonEntityForm(string solutionPath)
    {
      this._solutionPath = solutionPath;
      this.InitializeComponent();
    }

    private void JsonEntityForm_Load(object sender, EventArgs e)
    {
      this.PopulateEntityCombobox(this._solutionPath);
      this.cmbEntity.Enabled = true;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      try
      {
        ComboboxItem selectedItem = (ComboboxItem) this.cmbEntity.SelectedItem;
        if (selectedItem == null)
        {
          MsgBox.Warn("Select your entity!");
        }
        else
        {
          this.SelectedFile = selectedItem.Text;
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while selecting the entity!", ex);
      }
    }

    private void PopulateEntityCombobox(string path)
    {
      foreach (string jsonFile in JsonEntityForm.GetJsonFiles(path))
        this.cmbEntity.Items.Add((object) new ComboboxItem()
        {
          Text = jsonFile,
          Value = (object) jsonFile
        });
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
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (JsonEntityForm));
      this.cmbEntity = new ComboBox();
      this.btnOk = new Button();
      this.pnlBottom = new Panel();
      this.btnCancel = new Button();
      this.label1 = new Label();
      this.pnlBottom.SuspendLayout();
      this.SuspendLayout();
      this.cmbEntity.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbEntity.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbEntity.Enabled = false;
      this.cmbEntity.Font = new Font("Segoe UI", 9.75f);
      this.cmbEntity.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbEntity.FormattingEnabled = true;
      this.cmbEntity.Location = new Point(12, 49);
      this.cmbEntity.Name = "cmbEntity";
      this.cmbEntity.Size = new Size(475, 25);
      this.cmbEntity.TabIndex = 1;
      this.btnOk.BackColor = Color.FromArgb(3, 184, 120);
      this.btnOk.Cursor = Cursors.Hand;
      this.btnOk.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnOk.FlatStyle = FlatStyle.Flat;
      this.btnOk.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnOk.ForeColor = Color.White;
      this.btnOk.Location = new Point(362, 21);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new Size(125, 27);
      this.btnOk.TabIndex = 2;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = false;
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      this.pnlBottom.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlBottom.Controls.Add((Control) this.btnCancel);
      this.pnlBottom.Controls.Add((Control) this.btnOk);
      this.pnlBottom.Dock = DockStyle.Bottom;
      this.pnlBottom.Location = new Point(0, 108);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new Size(499, 60);
      this.pnlBottom.TabIndex = 2;
      this.btnCancel.BackColor = Color.Gray;
      this.btnCancel.Cursor = Cursors.Hand;
      this.btnCancel.DialogResult = DialogResult.Cancel;
      this.btnCancel.FlatAppearance.BorderColor = Color.Gray;
      this.btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatStyle = FlatStyle.Flat;
      this.btnCancel.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnCancel.ForeColor = Color.White;
      this.btnCancel.Location = new Point(274, 21);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new Size(64, 27);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = false;
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Segoe UI", 9.75f);
      this.label1.ForeColor = Color.FromArgb(221, 221, 221);
      this.label1.Location = new Point(12, 26);
      this.label1.Name = "label1";
      this.label1.Size = new Size(107, 17);
      this.label1.TabIndex = 6;
      this.label1.Text = "Select Your Entity";
      this.AcceptButton = (IButtonControl) this.btnOk;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(38, 38, 42);
      this.CancelButton = (IButtonControl) this.btnCancel;
      this.ClientSize = new Size(499, 168);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.pnlBottom);
      this.Controls.Add((Control) this.cmbEntity);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (JsonEntityForm);
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Entity";
      this.Load += new EventHandler(this.JsonEntityForm_Load);
      this.pnlBottom.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
