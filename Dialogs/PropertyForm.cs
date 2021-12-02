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
  public class PropertyForm : Form
  {
    public readonly Property Property;
    public readonly Entity Entity;
    public EnumDefinition ImportedEnum;
    public readonly List<string> ExistingPropertyNames;
    private string _originalName;
    private IContainer components;
    private Button btnOk;
    private Button btnCancel;
    private ComboBox cmbPropertyType;
    private TextBox txtPropertyName;
    private TextBox txtMaxLength;
    private CheckBox cbListInUi;
    private CheckBox cbCreateAndUpdate;
    private CheckBox cbRequired;
    private Label label1;
    private Label label2;
    private Label MaxLengthlbl;
    private TextBox txtMinLength;
    private TextBox txtRegularExpression;
    private Label Regexlbl;
    private CheckBox cbNullable;
    private Label label3;
    private CheckBox cbAdvancedFilter;
    private Panel pnlBottom;
    private Panel pnlPropertyInfo;
    private Label label7;
    private Label label4;
    private Panel pnlOthers;
    private Panel pnlAttributes;
    private Label label5;

    public PropertyForm(Entity entity, Property property, List<string> existingPropertyNames)
    {
      this.Property = property;
      this.ExistingPropertyNames = existingPropertyNames;
      this.Entity = entity;
      this.InitializeComponent();
    }

    private void PropertyForm_Load(object sender, EventArgs e) => this.Init();

    public void Init()
    {
      try
      {
        this._originalName = "";
        this.PopulatePropertyTypeCombobox();
        if (this.IsEditMode())
          this.LoadFromEditingProperty();
        this.ActiveControl = (Control) this.txtPropertyName;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error while initializing the form!", ex);
      }
    }

    private void LoadFromEditingProperty()
    {
      this._originalName = this.Property.Name;
      this.txtPropertyName.Text = this.Property.Name;
      if (!this.IsInCombobox(this.cmbPropertyType, this.Property.Type))
      {
        this.cmbPropertyType.Items.RemoveAt(this.cmbPropertyType.Items.Count - 1);
        this.cmbPropertyType.Items.Add((object) new ComboboxItem()
        {
          Text = this.Property.Type,
          Value = (object) this.Property.Type
        });
      }
      this.cmbPropertyType.Text = this.Property.Type;
      if (this.Property.Type.Equals("string"))
      {
        TextBox txtMaxLength = this.txtMaxLength;
        int num;
        string str1;
        if (this.Property.MaxLength != -1)
        {
          num = this.Property.MaxLength;
          str1 = num.ToString();
        }
        else
          str1 = "";
        txtMaxLength.Text = str1;
        TextBox txtMinLength = this.txtMinLength;
        string str2;
        if (this.Property.MinLength != -1)
        {
          num = this.Property.MinLength;
          str2 = num.ToString();
        }
        else
          str2 = "";
        txtMinLength.Text = str2;
        this.txtRegularExpression.Text = this.Property.Regex;
        this.cbRequired.Checked = this.Property.Required;
      }
      else if (this.Property.IsNumerical() && this.Property.Range.IsRangeSet)
      {
        this.txtMaxLength.Text = this.Property.Range.MaximumValue.ToString();
        this.txtMinLength.Text = this.Property.Range.MinimumValue.ToString();
      }
      this.cbCreateAndUpdate.Checked = this.Property.UserInterface.CreateOrUpdate;
      this.cbAdvancedFilter.Checked = this.Property.UserInterface.AdvancedFilter;
      this.cbListInUi.Checked = this.Property.UserInterface.List;
      this.cbNullable.Checked = this.Property.Nullable;
    }

    private bool IsEditMode() => !string.IsNullOrEmpty(this.Property.Name);

    private void PopulatePropertyTypeCombobox()
    {
      try
      {
        this.cmbPropertyType.Enabled = false;
        foreach (string propertyType in AppSettings.PropertyTypes)
          this.cmbPropertyType.Items.Add((object) new ComboboxItem()
          {
            Text = propertyType,
            Value = (object) propertyType
          });
        if (ProjectVersionHelper.ProjectVersionToNumber(this.Entity.ProjectVersion) >= 100300)
          this.cmbPropertyType.Items.Add((object) new ComboboxItem()
          {
            Text = "file",
            Value = (object) "file"
          });
        this.cmbPropertyType.SelectedIndex = 9;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while populating property type combobox!", ex);
      }
      finally
      {
        this.cmbPropertyType.Enabled = true;
      }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      try
      {
        if (!this.CheckValidation())
          return;
        this.Property.Name = this.txtPropertyName.Text;
        this.Property.Type = this.cmbPropertyType.Text;
        this.Property.UserInterface = new PropertyUserInterfaceInfo()
        {
          AdvancedFilter = this.cbAdvancedFilter.Checked,
          CreateOrUpdate = this.cbCreateAndUpdate.Checked,
          List = this.cbListInUi.Checked
        };
        if (this.Property.Type.Equals("string"))
        {
          try
          {
            this.Property.MaxLength = Convert.ToInt32(this.txtMaxLength.Text);
          }
          catch (Exception ex)
          {
            this.Property.MaxLength = -1;
          }
          try
          {
            this.Property.MinLength = Convert.ToInt32(this.txtMinLength.Text);
          }
          catch (Exception ex)
          {
            this.Property.MinLength = -1;
          }
        }
        this.Property.Regex = !this.Property.Type.Equals("string") || string.IsNullOrWhiteSpace(this.txtRegularExpression.Text) ? "" : this.txtRegularExpression.Text;
        this.Property.Required = this.Property.Type.Equals("string") && this.cbRequired.Checked;
        this.Property.Nullable = this.cbNullable.Checked;
        this.Property.Range = new NumericalRange();
        if (this.Property.IsDecimalNumber())
        {
          try
          {
            this.Property.Range.MinimumValue = Convert.ToDouble(this.txtMinLength.Text);
            this.Property.Range.MaximumValue = Convert.ToDouble(this.txtMaxLength.Text);
            this.Property.Range.IsRangeSet = true;
          }
          catch (Exception ex)
          {
            this.Property.Range.IsRangeSet = false;
          }
        }
        else if (this.Property.IsNumerical())
        {
          try
          {
            this.Property.Range.MinimumValue = (double) Convert.ToInt32(this.txtMinLength.Text);
            this.Property.Range.MaximumValue = (double) Convert.ToInt32(this.txtMaxLength.Text);
            this.Property.Range.IsRangeSet = true;
          }
          catch (Exception ex)
          {
            this.Property.Range.IsRangeSet = false;
          }
        }
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while creating the property!", ex);
      }
    }

    private void cmbPropertyType_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.cmbPropertyType.Text.Equals("enum"))
      {
        using (EnumSelectionForm enumSelectionForm = new EnumSelectionForm())
        {
          if (enumSelectionForm.ShowDialog() == DialogResult.OK)
          {
            this.ImportedEnum = enumSelectionForm.SelectedEnum;
            this.cmbPropertyType.Items.Remove((object) new ComboboxItem()
            {
              Text = "enum",
              Value = (object) "enum"
            });
            this.cmbPropertyType.Items.Add((object) new ComboboxItem()
            {
              Text = this.ImportedEnum.Name,
              Value = (object) this.ImportedEnum.Name
            });
            this.cmbPropertyType.SelectedIndex = this.cmbPropertyType.Items.Count - 1;
          }
        }
      }
      else if (this.cmbPropertyType.Text.Equals("string"))
      {
        this.txtRegularExpression.Enabled = true;
        this.cbRequired.Enabled = true;
        this.cbNullable.Enabled = false;
        this.txtMaxLength.Enabled = true;
        this.txtMinLength.Enabled = true;
        this.MaxLengthlbl.Text = "Min-Max Length";
      }
      else if (this.cmbPropertyType.Text.Equals("bool"))
      {
        this.txtMaxLength.Enabled = false;
        this.txtMinLength.Enabled = false;
        this.txtRegularExpression.Enabled = false;
        this.cbRequired.Enabled = false;
        this.cbNullable.Enabled = false;
      }
      else if (this.cmbPropertyType.Text.Equals("DateTime"))
      {
        this.txtMaxLength.Enabled = false;
        this.txtMinLength.Enabled = false;
        this.txtRegularExpression.Enabled = false;
        this.cbRequired.Enabled = false;
        this.cbNullable.Enabled = true;
      }
      else if (this.cmbPropertyType.Text.Equals("Guid"))
      {
        this.txtMaxLength.Enabled = false;
        this.txtMinLength.Enabled = false;
        this.txtRegularExpression.Enabled = false;
        this.cbRequired.Enabled = false;
        this.cbNullable.Enabled = false;
      }
      else if (this.cmbPropertyType.Text.Equals("file"))
      {
        this.txtMaxLength.Enabled = false;
        this.txtMinLength.Enabled = false;
        this.txtRegularExpression.Enabled = false;
        this.cbRequired.Enabled = false;
        this.cbNullable.Enabled = false;
        this.cbNullable.Checked = true;
      }
      else if (this.IsNumerical(this.cmbPropertyType.Text))
      {
        this.cbNullable.Enabled = true;
        this.txtRegularExpression.Enabled = false;
        this.cbRequired.Enabled = false;
        this.txtMaxLength.Enabled = true;
        this.txtMinLength.Enabled = true;
        this.MaxLengthlbl.Text = "Min-Max Value";
      }
      else
      {
        this.txtMaxLength.Enabled = false;
        this.txtMinLength.Enabled = false;
        this.txtRegularExpression.Enabled = false;
        this.cbRequired.Enabled = false;
        this.cbNullable.Enabled = false;
      }
      this.txtMaxLength.Text = "";
      this.txtMinLength.Text = "";
    }

    public bool CheckValidation()
    {
      this.txtPropertyName.Text = this.txtPropertyName.Text.Trim();
      if (string.IsNullOrWhiteSpace(this.txtPropertyName.Text) || this.txtPropertyName.Text.Contains(" ") || this.txtPropertyName.Text.Contains("-"))
      {
        MsgBox.Warn("Enter a valid property name!");
        return false;
      }
      if (this.ExistingPropertyNames.Any<string>((Func<string, bool>) (n => !this.txtPropertyName.Text.Equals(this._originalName) && n.Equals(this.txtPropertyName.Text))))
      {
        MsgBox.Warn("Property name already exists!");
        return false;
      }
      this.Property.Type = this.cmbPropertyType.Text;
      long result;
      if ((this.Property.IsNumerical() || this.Property.Type.Equals("string")) && long.TryParse(this.txtMinLength.Text, out result) != long.TryParse(this.txtMaxLength.Text, out result))
      {
        MsgBox.Warn("Set both max and min value!");
        return false;
      }
      if (this.Property.Type.Equals("string") || this.Property.IsNumerical())
      {
        try
        {
          if (Convert.ToInt32(this.txtMinLength.Text) > Convert.ToInt32(this.txtMaxLength.Text))
          {
            if (this.Property.Type.Equals("string"))
              MsgBox.Warn("Maximum length can not be less than minimum length!");
            else if (this.Property.IsNumerical())
              MsgBox.Warn("Maximum value can not be less than minimum value!");
            return false;
          }
        }
        catch (Exception ex)
        {
        }
      }
      return true;
    }

    private void btnCancel_Click(object sender, EventArgs e) => this.Close();

    public bool IsNumerical(string type) => type == "int" || type == "long" || type == "byte" || type == "short" || type == "double" || type == "decimal";

    public bool IsInCombobox(ComboBox cb, string text)
    {
      foreach (object obj in cb.Items)
      {
        if (obj.ToString().Equals(new ComboboxItem()
        {
          Text = text,
          Value = ((object) text)
        }.ToString()))
          return true;
      }
      return false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (PropertyForm));
      this.btnOk = new Button();
      this.btnCancel = new Button();
      this.cmbPropertyType = new ComboBox();
      this.txtPropertyName = new TextBox();
      this.txtMaxLength = new TextBox();
      this.cbListInUi = new CheckBox();
      this.cbCreateAndUpdate = new CheckBox();
      this.cbRequired = new CheckBox();
      this.label1 = new Label();
      this.label2 = new Label();
      this.MaxLengthlbl = new Label();
      this.label3 = new Label();
      this.txtRegularExpression = new TextBox();
      this.Regexlbl = new Label();
      this.cbNullable = new CheckBox();
      this.txtMinLength = new TextBox();
      this.cbAdvancedFilter = new CheckBox();
      this.pnlBottom = new Panel();
      this.pnlPropertyInfo = new Panel();
      this.label7 = new Label();
      this.label4 = new Label();
      this.pnlOthers = new Panel();
      this.pnlAttributes = new Panel();
      this.label5 = new Label();
      this.pnlBottom.SuspendLayout();
      this.pnlPropertyInfo.SuspendLayout();
      this.pnlOthers.SuspendLayout();
      this.pnlAttributes.SuspendLayout();
      this.SuspendLayout();
      this.btnOk.BackColor = Color.FromArgb(3, 184, 120);
      this.btnOk.Cursor = Cursors.Hand;
      this.btnOk.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnOk.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 184, 120);
      this.btnOk.FlatAppearance.MouseOverBackColor = Color.FromArgb(3, 184, 120);
      this.btnOk.FlatStyle = FlatStyle.Flat;
      this.btnOk.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnOk.ForeColor = Color.White;
      this.btnOk.Location = new Point(546, 18);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new Size((int) sbyte.MaxValue, 27);
      this.btnOk.TabIndex = 11;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = false;
      this.btnOk.Click += new EventHandler(this.btnOk_Click);
      this.btnCancel.BackColor = Color.Gray;
      this.btnCancel.Cursor = Cursors.Hand;
      this.btnCancel.DialogResult = DialogResult.Cancel;
      this.btnCancel.FlatAppearance.BorderColor = Color.Gray;
      this.btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatStyle = FlatStyle.Flat;
      this.btnCancel.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
      this.btnCancel.ForeColor = Color.White;
      this.btnCancel.Location = new Point(459, 18);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new Size(64, 27);
      this.btnCancel.TabIndex = 12;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = false;
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      this.cmbPropertyType.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbPropertyType.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbPropertyType.FlatStyle = FlatStyle.Flat;
      this.cmbPropertyType.Font = new Font("Segoe UI", 9.75f);
      this.cmbPropertyType.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbPropertyType.FormattingEnabled = true;
      this.cmbPropertyType.Location = new Point(133, 22);
      this.cmbPropertyType.Name = "cmbPropertyType";
      this.cmbPropertyType.RightToLeft = RightToLeft.No;
      this.cmbPropertyType.Size = new Size(160, 25);
      this.cmbPropertyType.TabIndex = 1;
      this.cmbPropertyType.SelectedIndexChanged += new EventHandler(this.cmbPropertyType_SelectedIndexChanged);
      this.txtPropertyName.BackColor = Color.FromArgb(224, 224, 224);
      this.txtPropertyName.BorderStyle = BorderStyle.FixedSingle;
      this.txtPropertyName.Font = new Font("Segoe UI", 9.75f);
      this.txtPropertyName.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtPropertyName.Location = new Point(133, 73);
      this.txtPropertyName.Name = "txtPropertyName";
      this.txtPropertyName.RightToLeft = RightToLeft.No;
      this.txtPropertyName.Size = new Size(160, 25);
      this.txtPropertyName.TabIndex = 2;
      this.txtMaxLength.BackColor = Color.FromArgb(224, 224, 224);
      this.txtMaxLength.BorderStyle = BorderStyle.FixedSingle;
      this.txtMaxLength.Font = new Font("Segoe UI", 9.75f);
      this.txtMaxLength.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtMaxLength.Location = new Point(228, 27);
      this.txtMaxLength.Name = "txtMaxLength";
      this.txtMaxLength.RightToLeft = RightToLeft.No;
      this.txtMaxLength.Size = new Size(58, 25);
      this.txtMaxLength.TabIndex = 7;
      this.cbListInUi.AutoSize = true;
      this.cbListInUi.Checked = true;
      this.cbListInUi.CheckState = CheckState.Checked;
      this.cbListInUi.Cursor = Cursors.Hand;
      this.cbListInUi.Font = new Font("Segoe UI", 9.75f);
      this.cbListInUi.ForeColor = SystemColors.ControlLightLight;
      this.cbListInUi.Location = new Point(21, 49);
      this.cbListInUi.Name = "cbListInUi";
      this.cbListInUi.RightToLeft = RightToLeft.No;
      this.cbListInUi.Size = new Size(80, 21);
      this.cbListInUi.TabIndex = 4;
      this.cbListInUi.Text = "List in UI ";
      this.cbListInUi.UseVisualStyleBackColor = true;
      this.cbCreateAndUpdate.AutoSize = true;
      this.cbCreateAndUpdate.Checked = true;
      this.cbCreateAndUpdate.CheckState = CheckState.Checked;
      this.cbCreateAndUpdate.Cursor = Cursors.Hand;
      this.cbCreateAndUpdate.Font = new Font("Segoe UI", 9.75f);
      this.cbCreateAndUpdate.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbCreateAndUpdate.Location = new Point(21, 78);
      this.cbCreateAndUpdate.Name = "cbCreateAndUpdate";
      this.cbCreateAndUpdate.RightToLeft = RightToLeft.No;
      this.cbCreateAndUpdate.Size = new Size(139, 21);
      this.cbCreateAndUpdate.TabIndex = 5;
      this.cbCreateAndUpdate.Text = "Create And Update";
      this.cbCreateAndUpdate.UseVisualStyleBackColor = true;
      this.cbRequired.AutoSize = true;
      this.cbRequired.Cursor = Cursors.Hand;
      this.cbRequired.Font = new Font("Segoe UI", 9.75f);
      this.cbRequired.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbRequired.Location = new Point(56, (int) sbyte.MaxValue);
      this.cbRequired.Name = "cbRequired";
      this.cbRequired.RightToLeft = RightToLeft.Yes;
      this.cbRequired.Size = new Size(92, 21);
      this.cbRequired.TabIndex = 9;
      this.cbRequired.Text = "  Required ";
      this.cbRequired.UseVisualStyleBackColor = true;
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Segoe UI", 9.75f);
      this.label1.ForeColor = Color.FromArgb(221, 221, 221);
      this.label1.Location = new Point(12, 25);
      this.label1.Name = "label1";
      this.label1.Size = new Size(89, 17);
      this.label1.TabIndex = 9;
      this.label1.Text = "Property Type";
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Segoe UI", 9.75f);
      this.label2.ForeColor = Color.FromArgb(221, 221, 221);
      this.label2.Location = new Point(12, 76);
      this.label2.Name = "label2";
      this.label2.Size = new Size(97, 17);
      this.label2.TabIndex = 10;
      this.label2.Text = "Property Name";
      this.MaxLengthlbl.AutoSize = true;
      this.MaxLengthlbl.Font = new Font("Segoe UI", 9.75f);
      this.MaxLengthlbl.ForeColor = Color.FromArgb(221, 221, 221);
      this.MaxLengthlbl.Location = new Point(7, 31);
      this.MaxLengthlbl.Name = "MaxLengthlbl";
      this.MaxLengthlbl.Size = new Size(103, 17);
      this.MaxLengthlbl.TabIndex = 11;
      this.MaxLengthlbl.Text = "Min-Max Length";
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Segoe UI", 24f);
      this.label3.ForeColor = Color.FromArgb(221, 221, 221);
      this.label3.Location = new Point(195, 14);
      this.label3.Name = "label3";
      this.label3.RightToLeft = RightToLeft.Yes;
      this.label3.Size = new Size(33, 45);
      this.label3.TabIndex = 21;
      this.label3.Text = "-";
      this.label3.TextAlign = ContentAlignment.MiddleCenter;
      this.txtRegularExpression.BackColor = Color.FromArgb(224, 224, 224);
      this.txtRegularExpression.BorderStyle = BorderStyle.FixedSingle;
      this.txtRegularExpression.Font = new Font("Segoe UI", 9.75f);
      this.txtRegularExpression.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtRegularExpression.Location = new Point(136, 74);
      this.txtRegularExpression.Name = "txtRegularExpression";
      this.txtRegularExpression.RightToLeft = RightToLeft.No;
      this.txtRegularExpression.Size = new Size(150, 25);
      this.txtRegularExpression.TabIndex = 8;
      this.Regexlbl.AutoSize = true;
      this.Regexlbl.Font = new Font("Segoe UI", 9.75f);
      this.Regexlbl.ForeColor = Color.FromArgb(221, 221, 221);
      this.Regexlbl.Location = new Point(10, 76);
      this.Regexlbl.Name = "Regexlbl";
      this.Regexlbl.RightToLeft = RightToLeft.Yes;
      this.Regexlbl.Size = new Size(120, 17);
      this.Regexlbl.TabIndex = 20;
      this.Regexlbl.Text = "Regular Expression";
      this.cbNullable.AutoSize = true;
      this.cbNullable.Cursor = Cursors.Hand;
      this.cbNullable.Font = new Font("Segoe UI", 9.75f);
      this.cbNullable.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbNullable.Location = new Point(65, 162);
      this.cbNullable.Name = "cbNullable";
      this.cbNullable.RightToLeft = RightToLeft.Yes;
      this.cbNullable.Size = new Size(83, 21);
      this.cbNullable.TabIndex = 10;
      this.cbNullable.Text = "  Nullable";
      this.cbNullable.UseVisualStyleBackColor = true;
      this.txtMinLength.BackColor = Color.FromArgb(224, 224, 224);
      this.txtMinLength.BorderStyle = BorderStyle.FixedSingle;
      this.txtMinLength.Font = new Font("Segoe UI", 9.75f);
      this.txtMinLength.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtMinLength.Location = new Point(136, 27);
      this.txtMinLength.Name = "txtMinLength";
      this.txtMinLength.RightToLeft = RightToLeft.No;
      this.txtMinLength.Size = new Size(58, 25);
      this.txtMinLength.TabIndex = 6;
      this.cbAdvancedFilter.AutoSize = true;
      this.cbAdvancedFilter.Checked = true;
      this.cbAdvancedFilter.CheckState = CheckState.Checked;
      this.cbAdvancedFilter.Cursor = Cursors.Hand;
      this.cbAdvancedFilter.Font = new Font("Segoe UI", 9.75f);
      this.cbAdvancedFilter.ForeColor = SystemColors.ControlLightLight;
      this.cbAdvancedFilter.Location = new Point(21, 20);
      this.cbAdvancedFilter.Name = "cbAdvancedFilter";
      this.cbAdvancedFilter.RightToLeft = RightToLeft.No;
      this.cbAdvancedFilter.Size = new Size(116, 21);
      this.cbAdvancedFilter.TabIndex = 3;
      this.cbAdvancedFilter.Text = "Advanced Filter";
      this.cbAdvancedFilter.UseVisualStyleBackColor = true;
      this.pnlBottom.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlBottom.Controls.Add((Control) this.btnCancel);
      this.pnlBottom.Controls.Add((Control) this.btnOk);
      this.pnlBottom.Font = new Font("Microsoft Sans Serif", 10f);
      this.pnlBottom.Location = new Point(-4, 325);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new Size(736, 60);
      this.pnlBottom.TabIndex = 15;
      this.pnlPropertyInfo.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlPropertyInfo.Controls.Add((Control) this.cmbPropertyType);
      this.pnlPropertyInfo.Controls.Add((Control) this.label1);
      this.pnlPropertyInfo.Controls.Add((Control) this.txtPropertyName);
      this.pnlPropertyInfo.Controls.Add((Control) this.label2);
      this.pnlPropertyInfo.Location = new Point(21, 33);
      this.pnlPropertyInfo.Name = "pnlPropertyInfo";
      this.pnlPropertyInfo.Size = new Size(312, 125);
      this.pnlPropertyInfo.TabIndex = 16;
      this.label7.AutoSize = true;
      this.label7.Font = new Font("Segoe UI", 11f);
      this.label7.ForeColor = Color.White;
      this.label7.Location = new Point(17, 10);
      this.label7.Name = "label7";
      this.label7.Size = new Size(95, 20);
      this.label7.TabIndex = 18;
      this.label7.Text = "Property Info";
      this.label4.AutoSize = true;
      this.label4.Font = new Font("Segoe UI", 11f);
      this.label4.ForeColor = Color.White;
      this.label4.Location = new Point(17, 170);
      this.label4.Name = "label4";
      this.label4.Size = new Size(52, 20);
      this.label4.TabIndex = 19;
      this.label4.Text = "Others";
      this.pnlOthers.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlOthers.Controls.Add((Control) this.cbAdvancedFilter);
      this.pnlOthers.Controls.Add((Control) this.cbCreateAndUpdate);
      this.pnlOthers.Controls.Add((Control) this.cbListInUi);
      this.pnlOthers.Location = new Point(21, 195);
      this.pnlOthers.Name = "pnlOthers";
      this.pnlOthers.Size = new Size(312, 112);
      this.pnlOthers.TabIndex = 20;
      this.pnlAttributes.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlAttributes.Controls.Add((Control) this.MaxLengthlbl);
      this.pnlAttributes.Controls.Add((Control) this.txtRegularExpression);
      this.pnlAttributes.Controls.Add((Control) this.txtMaxLength);
      this.pnlAttributes.Controls.Add((Control) this.Regexlbl);
      this.pnlAttributes.Controls.Add((Control) this.cbRequired);
      this.pnlAttributes.Controls.Add((Control) this.cbNullable);
      this.pnlAttributes.Controls.Add((Control) this.txtMinLength);
      this.pnlAttributes.Controls.Add((Control) this.label3);
      this.pnlAttributes.Location = new Point(357, 33);
      this.pnlAttributes.Name = "pnlAttributes";
      this.pnlAttributes.Size = new Size(312, 274);
      this.pnlAttributes.TabIndex = 22;
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Segoe UI", 11f);
      this.label5.ForeColor = Color.White;
      this.label5.Location = new Point(353, 6);
      this.label5.Name = "label5";
      this.label5.Size = new Size(74, 20);
      this.label5.TabIndex = 21;
      this.label5.Text = "Attributes";
      this.AcceptButton = (IButtonControl) this.btnOk;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(38, 38, 42);
      this.CancelButton = (IButtonControl) this.btnCancel;
      this.ClientSize = new Size(693, 382);
      this.Controls.Add((Control) this.pnlAttributes);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.pnlOthers);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.label7);
      this.Controls.Add((Control) this.pnlPropertyInfo);
      this.Controls.Add((Control) this.pnlBottom);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (PropertyForm);
      this.RightToLeft = RightToLeft.No;
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Property";
      this.Load += new EventHandler(this.PropertyForm_Load);
      this.pnlBottom.ResumeLayout(false);
      this.pnlPropertyInfo.ResumeLayout(false);
      this.pnlPropertyInfo.PerformLayout();
      this.pnlOthers.ResumeLayout(false);
      this.pnlOthers.PerformLayout();
      this.pnlAttributes.ResumeLayout(false);
      this.pnlAttributes.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
