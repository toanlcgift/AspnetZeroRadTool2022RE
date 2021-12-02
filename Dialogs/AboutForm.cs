using AspNetZeroRadToolVisualStudioExtension.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs
{
  public class AboutForm : Form
  {
    private readonly Action _windowClosedCallback;
    private IContainer components;
    private Panel pnlBottom;
    private Button btnClose;
    private Label label2;
    private Label lblVersion;
    private Button btnOpenLogs;
    private LinkLabel lnkHowToUseIt;
    private LinkLabel lnkAspNetZero;
    private Label label4;
    private Label label5;
    private PictureBox pictureBox1;
    private Button btnSaveLogs;
    private LinkLabel lnkSupportAspNetZero;
    private Label label1;

    public AboutForm(Action windowClosedCallback)
    {
      this.InitializeComponent();
      this._windowClosedCallback = windowClosedCallback;
    }

    private void AboutForm_Load(object sender, EventArgs e) => this.lblVersion.Text = AppSettings.GetAppVersion();

    private void btnOpenLogs_Click(object sender, EventArgs e)
    {
      try
      {
        string logFilePathOrNull = Logger.GetLogFilePathOrNull();
        if (logFilePathOrNull == null)
          MsgBox.Warn("Cannot find log file directory!");
        else if (!File.Exists(logFilePathOrNull))
          MsgBox.Warn("No logs yet!");
        else
          Process.Start(logFilePathOrNull);
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while opening the log file!", ex);
      }
    }

    private void btnSaveLogs_Click(object sender, EventArgs e)
    {
      try
      {
        string logFilePathOrNull = Logger.GetLogFilePathOrNull();
        if (logFilePathOrNull == null)
        {
          MsgBox.Warn("Cannot find log file directory!");
        }
        else
        {
          DirectoryInfo directory = new FileInfo(logFilePathOrNull).Directory;
          if (directory == null || !directory.Exists || !((IEnumerable<FileInfo>) directory.GetFiles()).Any<FileInfo>())
          {
            MsgBox.Warn("No log folder yet!");
          }
          else
          {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
              ShowNewFolderButton = true,
              Description = "Select a folder to save the logs zip file."
            };
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
              return;
            string destinationArchiveFileName = Path.Combine(folderBrowserDialog.SelectedPath, "PowerTools_" + AppSettings.GetAppVersion() + "_Logs.zip");
            ZipFile.CreateFromDirectory(directory.FullName, destinationArchiveFileName);
            MsgBox.Info("Saved to " + destinationArchiveFileName);
          }
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while saving the log file!", ex);
      }
    }

    private void btnClose_Click(object sender, EventArgs e) => this.Close();

    private void lnkAspNetZero_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://aspnetzero.com/");

    private void lnkHowToUseIt_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://docs.aspnetzero.com/documents/zero/latest/Development-Guide-Rad-Tool");

    private void lnkSupportAspNetZero_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://support.aspnetzero.com/");

    public void AboutForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Action windowClosedCallback = this._windowClosedCallback;
      if (windowClosedCallback == null)
        return;
      windowClosedCallback();
    }

    private void lblVersion_Click(object sender, EventArgs e)
    {
      try
      {
        if (Control.ModifierKeys != Keys.Control)
          return;
        string installationDirectoryOrNull = AppSettings.GetExtensionInstallationDirectoryOrNull();
        if (installationDirectoryOrNull == null)
        {
          MsgBox.Exception("Cannot get extension directory!");
        }
        else
        {
          if (!Directory.Exists(installationDirectoryOrNull))
            return;
          Process.Start("explorer.exe", installationDirectoryOrNull);
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Cannot open extension installation directory!", ex);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (AboutForm));
      this.pnlBottom = new Panel();
      this.btnSaveLogs = new Button();
      this.btnOpenLogs = new Button();
      this.btnClose = new Button();
      this.label2 = new Label();
      this.lblVersion = new Label();
      this.lnkHowToUseIt = new LinkLabel();
      this.lnkAspNetZero = new LinkLabel();
      this.label4 = new Label();
      this.label5 = new Label();
      this.pictureBox1 = new PictureBox();
      this.lnkSupportAspNetZero = new LinkLabel();
      this.label1 = new Label();
      this.pnlBottom.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.pnlBottom.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlBottom.Controls.Add((Control) this.btnSaveLogs);
      this.pnlBottom.Controls.Add((Control) this.btnOpenLogs);
      this.pnlBottom.Controls.Add((Control) this.btnClose);
      this.pnlBottom.Dock = DockStyle.Bottom;
      this.pnlBottom.Location = new Point(0, 178);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new Size(451, 60);
      this.pnlBottom.TabIndex = 3;
      this.btnSaveLogs.BackColor = Color.DarkGray;
      this.btnSaveLogs.Cursor = Cursors.Hand;
      this.btnSaveLogs.DialogResult = DialogResult.Cancel;
      this.btnSaveLogs.FlatAppearance.BorderColor = Color.Gray;
      this.btnSaveLogs.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 184, 120);
      this.btnSaveLogs.FlatAppearance.MouseOverBackColor = Color.FromArgb(3, 184, 120);
      this.btnSaveLogs.FlatStyle = FlatStyle.Flat;
      this.btnSaveLogs.Font = new Font("Segoe UI", 10f);
      this.btnSaveLogs.ForeColor = Color.White;
      this.btnSaveLogs.Location = new Point(15, 21);
      this.btnSaveLogs.Name = "btnSaveLogs";
      this.btnSaveLogs.Size = new Size(100, 28);
      this.btnSaveLogs.TabIndex = 5;
      this.btnSaveLogs.Text = "Save Logs";
      this.btnSaveLogs.UseVisualStyleBackColor = false;
      this.btnSaveLogs.Click += new EventHandler(this.btnSaveLogs_Click);
      this.btnOpenLogs.BackColor = Color.DarkGray;
      this.btnOpenLogs.Cursor = Cursors.Hand;
      this.btnOpenLogs.DialogResult = DialogResult.Cancel;
      this.btnOpenLogs.FlatAppearance.BorderColor = Color.Gray;
      this.btnOpenLogs.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 184, 120);
      this.btnOpenLogs.FlatAppearance.MouseOverBackColor = Color.FromArgb(3, 184, 120);
      this.btnOpenLogs.FlatStyle = FlatStyle.Flat;
      this.btnOpenLogs.Font = new Font("Segoe UI", 10f);
      this.btnOpenLogs.ForeColor = Color.White;
      this.btnOpenLogs.Location = new Point(135, 21);
      this.btnOpenLogs.Name = "btnOpenLogs";
      this.btnOpenLogs.Size = new Size(100, 28);
      this.btnOpenLogs.TabIndex = 4;
      this.btnOpenLogs.Text = "Open Logs";
      this.btnOpenLogs.UseVisualStyleBackColor = false;
      this.btnOpenLogs.Click += new EventHandler(this.btnOpenLogs_Click);
      this.btnClose.BackColor = Color.Gray;
      this.btnClose.Cursor = Cursors.Hand;
      this.btnClose.DialogResult = DialogResult.Cancel;
      this.btnClose.FlatAppearance.BorderColor = Color.Gray;
      this.btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 184, 120);
      this.btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(3, 184, 120);
      this.btnClose.FlatStyle = FlatStyle.Flat;
      this.btnClose.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnClose.ForeColor = Color.White;
      this.btnClose.Location = new Point(374, 21);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new Size(64, 27);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = false;
      this.btnClose.Click += new EventHandler(this.btnClose_Click);
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Segoe UI", 9.75f);
      this.label2.ForeColor = Color.FromArgb(221, 221, 221);
      this.label2.Location = new Point(12, 85);
      this.label2.Name = "label2";
      this.label2.Size = new Size(54, 17);
      this.label2.TabIndex = 8;
      this.label2.Text = "Version:";
      this.lblVersion.AutoSize = true;
      this.lblVersion.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
      this.lblVersion.ForeColor = Color.FromArgb(221, 221, 221);
      this.lblVersion.Location = new Point(72, 85);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new Size(13, 17);
      this.lblVersion.TabIndex = 9;
      this.lblVersion.Text = "-";
      this.lblVersion.Click += new EventHandler(this.lblVersion_Click);
      this.lnkHowToUseIt.ActiveLinkColor = Color.FromArgb(3, 184, 120);
      this.lnkHowToUseIt.AutoSize = true;
      this.lnkHowToUseIt.BackColor = Color.Transparent;
      this.lnkHowToUseIt.Font = new Font("Segoe UI", 9.75f);
      this.lnkHowToUseIt.ForeColor = Color.FromArgb(221, 221, 221);
      this.lnkHowToUseIt.LinkColor = Color.FromArgb(221, 221, 221);
      this.lnkHowToUseIt.Location = new Point(239, 85);
      this.lnkHowToUseIt.Name = "lnkHowToUseIt";
      this.lnkHowToUseIt.Size = new Size(174, 17);
      this.lnkHowToUseIt.TabIndex = 10;
      this.lnkHowToUseIt.TabStop = true;
      this.lnkHowToUseIt.Text = "https://docs.aspnetzero.com";
      this.lnkHowToUseIt.VisitedLinkColor = Color.FromArgb(3, 184, 120);
      this.lnkHowToUseIt.LinkClicked += new LinkLabelLinkClickedEventHandler(this.lnkHowToUseIt_LinkClicked);
      this.lnkAspNetZero.ActiveLinkColor = Color.FromArgb(3, 184, 120);
      this.lnkAspNetZero.AutoSize = true;
      this.lnkAspNetZero.BackColor = Color.Transparent;
      this.lnkAspNetZero.Font = new Font("Segoe UI", 9.75f);
      this.lnkAspNetZero.ForeColor = Color.FromArgb(221, 221, 221);
      this.lnkAspNetZero.LinkColor = Color.FromArgb(221, 221, 221);
      this.lnkAspNetZero.Location = new Point(239, 31);
      this.lnkAspNetZero.Name = "lnkAspNetZero";
      this.lnkAspNetZero.Size = new Size(143, 17);
      this.lnkAspNetZero.TabIndex = 11;
      this.lnkAspNetZero.TabStop = true;
      this.lnkAspNetZero.Text = "https://aspnetzero.com";
      this.lnkAspNetZero.VisitedLinkColor = Color.FromArgb(3, 184, 120);
      this.lnkAspNetZero.LinkClicked += new LinkLabelLinkClickedEventHandler(this.lnkAspNetZero_LinkClicked);
      this.label4.AutoSize = true;
      this.label4.Font = new Font("Segoe UI", 9.75f);
      this.label4.ForeColor = Color.FromArgb(221, 221, 221);
      this.label4.Location = new Point(239, 14);
      this.label4.Name = "label4";
      this.label4.Size = new Size(57, 17);
      this.label4.TabIndex = 12;
      this.label4.Text = "Website:";
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Segoe UI", 9.75f);
      this.label5.ForeColor = Color.FromArgb(221, 221, 221);
      this.label5.Location = new Point(239, 68);
      this.label5.Name = "label5";
      this.label5.Size = new Size(91, 17);
      this.label5.TabIndex = 13;
      this.label5.Text = "How to use it?";
      this.pictureBox1.Image = (Image) componentResourceManager.GetObject("pictureBox1.Image");
      this.pictureBox1.Location = new Point(12, 14);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(150, 51);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 15;
      this.pictureBox1.TabStop = false;
      this.lnkSupportAspNetZero.ActiveLinkColor = Color.FromArgb(3, 184, 120);
      this.lnkSupportAspNetZero.AutoSize = true;
      this.lnkSupportAspNetZero.BackColor = Color.Transparent;
      this.lnkSupportAspNetZero.Font = new Font("Segoe UI", 9.75f);
      this.lnkSupportAspNetZero.ForeColor = Color.FromArgb(221, 221, 221);
      this.lnkSupportAspNetZero.LinkColor = Color.FromArgb(221, 221, 221);
      this.lnkSupportAspNetZero.Location = new Point(239, 138);
      this.lnkSupportAspNetZero.Name = "lnkSupportAspNetZero";
      this.lnkSupportAspNetZero.Size = new Size(192, 17);
      this.lnkSupportAspNetZero.TabIndex = 16;
      this.lnkSupportAspNetZero.TabStop = true;
      this.lnkSupportAspNetZero.Text = "https://support.aspnetzero.com";
      this.lnkSupportAspNetZero.VisitedLinkColor = Color.FromArgb(3, 184, 120);
      this.lnkSupportAspNetZero.LinkClicked += new LinkLabelLinkClickedEventHandler(this.lnkSupportAspNetZero_LinkClicked);
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Segoe UI", 9.75f);
      this.label1.ForeColor = Color.FromArgb(221, 221, 221);
      this.label1.Location = new Point(239, 121);
      this.label1.Name = "label1";
      this.label1.Size = new Size(75, 17);
      this.label1.TabIndex = 17;
      this.label1.Text = "Need help?";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(38, 38, 42);
      this.CancelButton = (IButtonControl) this.btnClose;
      this.ClientSize = new Size(451, 238);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.lnkSupportAspNetZero);
      this.Controls.Add((Control) this.pictureBox1);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.lnkAspNetZero);
      this.Controls.Add((Control) this.lnkHowToUseIt);
      this.Controls.Add((Control) this.lblVersion);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.pnlBottom);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (AboutForm);
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "About Power Tools";
      this.FormClosed += new FormClosedEventHandler(this.AboutForm_FormClosed);
      this.Load += new EventHandler(this.AboutForm_Load);
      this.pnlBottom.ResumeLayout(false);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
