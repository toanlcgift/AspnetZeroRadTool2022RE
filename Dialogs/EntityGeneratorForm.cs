using AspNetZeroRadToolVisualStudioExtension.Dialogs.Providers;
using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using AspNetZeroRadToolVisualStudioExtension.Helpers;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs
{
  public class EntityGeneratorForm : Form
  {
    public Entity Entity;
    public List<Panel> PropertyBoxes;
    public List<Panel> NavigationPropertyBoxes;
    public List<Panel> NavigationPropertyOneToManyTableBoxes;
    public string RootPath;
    public bool AllowNoneMultiTenancy;
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private bool _mustSelectDbContext;
    private List<DbContextDefinition> _allDbContexts;
    protected int NewGroupBoxX = 780;
    protected int NewGroupBoxY = 38;
    private IContainer components;
    private TabControl tabContainer;
    private TabPage EntityInfoTab;
    private TabPage NavigationPropertyOneToManyTab;
    private TabPage tabPage2;
    private Label Namespacelbl;
    private TextBox txtRelativeNamespace;
    private CheckBox cbUpdateDatabase;
    private CheckBox cbAddMigration;
    private Label label5;
    private ComboBox cmbPrimaryKey;
    private Label label4;
    private TextBox txtEntityName;
    private ComboBox cmbBaseClass;
    private Label label1;
    private Label label3;
    private TextBox txtTableName;
    private Label label2;
    private CheckBox cbCreateUserInterface;
    private CheckBox cbTenant;
    private CheckBox cbHost;
    private Button btnGenerate;
    private Label namespace2lbl;
    private TextBox txtEntityNamePlural;
    private Label label6;
    private ComboBox cbMenuPosition;
    private Panel PropertiesPanel;
    private Button btnAddProperty;
    private TabPage NavigationTab;
    private Panel NavigationPropertiesPanel;
    private Button btnAddNavigationProperty;
    private CheckBox cbCreateViewOnlyModal;
    private CheckBox cbExcelExport;
    private CheckBox cbEntityHistory;
    private Panel panelTop;
    private Button btnEntityInformation;
    private Button btnNavigationProperties;
    private Button btnProperties;
    private Label label7;
    private Panel pnlEntityInfo;
    private Panel pnlBottom;
    private Label label10;
    private Panel pnlMultiTenancy;
    private Label label9;
    private Panel pnlAuditing;
    private Label label8;
    private Panel pnlDatabaseMigrations;
    private Label label11;
    private Panel pnlUserInterface;
    private Panel panel8;
    private Panel panel11;
    private Panel panel10;
    private Panel panel9;
    private Panel panel12;
    private Label label13;
    private Label label12;
    private Panel panel13;
    private Label label14;
    private Label label15;
    private Label EntityHistoryDisabledLabel;
    private Panel panel3;
    private Button btnCancel;
    private ComboBox cmbDbContextSelect;
    private CheckBox cbNonModalCRUDPage;
    private Panel panel1;
    private Button btnNavigationPropertyOneToManyTab;
    private Panel panel2;
    private Button btnAddNavigationPropertyOneToManyTable;
    private Label label17;
    private Panel NavigationPropertyOneToManyTablesPanel;
    private CheckBox cbMasterDetailPage;
    private Panel pnlTabBtns;
    private Label label16;

    public EntityGeneratorForm(Entity entity)
    {
      this.Entity = entity;
      this.InitializeComponent();
    }

    private void EntityGeneratorForm_Load(object sender, EventArgs e)
    {
      this.SetDefaultValues();
      this.RootPath = this.Entity.RootPath;
      this.PropertyBoxes = new List<Panel>();
      this.NavigationPropertyBoxes = new List<Panel>();
      this.NavigationPropertyOneToManyTableBoxes = new List<Panel>();
      if (this.Entity.IsRegenerate || this.Entity.IsLoadedFromDatabase)
        this.SetLoadedEntity();
      this.txtRelativeNamespace.Focus();
    }

    private void SetDefaultValues()
    {
      this.PopulateBaseClassCombobox();
      this.PopulatePrimaryKeyCombobox();
      this.PopulateMenuPositionCombobox();
      this.PopulateDbContextsCombobox();
      this.namespace2lbl.Text = this.Entity.Namespace;
      if (this.Entity.EntityHistoryDisabled)
      {
        this.cbEntityHistory.Enabled = false;
        this.EntityHistoryDisabledLabel.Text = "Your project doesn't support this feature!";
      }
      this.SetMultitenancyCheckboxes();
      if (this.Entity.IsRegenerate)
      {
        this.txtRelativeNamespace.Enabled = false;
        this.txtEntityName.Enabled = false;
        this.txtEntityNamePlural.Enabled = false;
      }
      this.ActiveControl = (Control) this.txtRelativeNamespace;
    }

    private void SetMultitenancyCheckboxes()
    {
      try
      {
        if (MultiTenancyChecker.IsEnabled(this.Entity))
          return;
        this.AllowNoneMultiTenancy = true;
        this.cbHost.Enabled = false;
        this.cbHost.Checked = false;
        this.cbTenant.Enabled = false;
        this.cbTenant.Checked = false;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Cannot determine multi-tenancy!", ex);
      }
    }

    private void PopulateMenuPositionCombobox()
    {
      this.cbMenuPosition.Items.Add((object) new ComboboxItem()
      {
        Text = "Root",
        Value = (object) "Root"
      });
      this.cbMenuPosition.Items.Add((object) new ComboboxItem()
      {
        Text = "Administration",
        Value = (object) "Administration"
      });
      this.cbMenuPosition.SelectedIndex = 0;
    }

    private void PopulatePrimaryKeyCombobox()
    {
      this.cmbPrimaryKey.Items.Add((object) new ComboboxItem()
      {
        Text = "Guid",
        Value = (object) "Guid"
      });
      this.cmbPrimaryKey.Items.Add((object) new ComboboxItem()
      {
        Text = "int",
        Value = (object) "int"
      });
      this.cmbPrimaryKey.Items.Add((object) new ComboboxItem()
      {
        Text = "long",
        Value = (object) "long"
      });
      this.cmbPrimaryKey.Items.Add((object) new ComboboxItem()
      {
        Text = "string",
        Value = (object) "string"
      });
      this.cmbPrimaryKey.SelectedIndex = 1;
    }

    private void PopulateBaseClassCombobox()
    {
      this.cmbBaseClass.Items.Add((object) new ComboboxItem()
      {
        Text = "Entity",
        Value = (object) "Entity"
      });
      this.cmbBaseClass.Items.Add((object) new ComboboxItem()
      {
        Text = "AuditedEntity",
        Value = (object) "AuditedEntity"
      });
      this.cmbBaseClass.Items.Add((object) new ComboboxItem()
      {
        Text = "CreationAuditedEntity",
        Value = (object) "CreationAuditedEntity"
      });
      this.cmbBaseClass.Items.Add((object) new ComboboxItem()
      {
        Text = "FullAuditedEntity",
        Value = (object) "FullAuditedEntity"
      });
      this.cmbBaseClass.SelectedIndex = 0;
    }

    private bool Generate(string folderPath)
    {
      try
      {
        if (!new DirectoryInfo(folderPath).Exists)
        {
          MsgBox.Warn("Directoy doesn't exist " + folderPath);
          return false;
        }
        string str1 = new DriveInfo(folderPath).Name.TrimEnd('\\');
        string str2 = this.Entity.RelativeNamespace + "." + this.Entity.EntityName + ".json";
        string str3 = "/C " + str1 + " && cd \"" + folderPath + "\" && dotnet AspNetZeroRadTool.dll " + str2;
        Process process = new Process();
        process.StartInfo = new ProcessStartInfo()
        {
          FileName = "cmd.exe",
          Arguments = str3
        };
        process.Start();
        process.WaitForExit();
        process.Dispose();
        return true;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while executing AspNetZeroRadTool.dll!", ex);
        return false;
      }
    }

    private bool WriteEntityJsonFile()
    {
      try
      {
        this.BuildEntity();
        string str = Path.Combine(this.RootPath, "AspNetZeroRadTool");
        if (!Directory.Exists(str))
        {
          MsgBox.Warn("Directory doesn't exist " + str);
          return false;
        }
        string path2 = this.Entity.RelativeNamespace + "." + this.Entity.EntityName + ".json";
        string path = Path.Combine(str, path2);
        try
        {
          File.WriteAllText(path, JsonConvert.SerializeObject((object) this.Entity, Formatting.Indented));
        }
        catch (Exception ex)
        {
          MsgBox.Exception("Cannot build the JSON file " + path, ex);
          return false;
        }
        return true;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error creating entity JSON file!", ex);
        return false;
      }
    }

    private void BuildEntity()
    {
      this.Entity.RelativeNamespace = EntityGeneratorForm.Upper(this.txtRelativeNamespace.Text);
      this.Entity.TableName = this.txtTableName.Text;
      this.Entity.EntityName = EntityGeneratorForm.Upper(this.txtEntityName.Text);
      this.Entity.EntityNamePlural = EntityGeneratorForm.Upper(this.txtEntityNamePlural.Text);
      this.Entity.BaseClass = this.cmbBaseClass.Text;
      this.Entity.PrimaryKeyType = this.cmbPrimaryKey.Text;
      this.Entity.EntityHistory = this.cbEntityHistory.Checked;
      this.Entity.AutoMigration = this.cbAddMigration.Checked;
      this.Entity.UpdateDatabase = this.cbUpdateDatabase.Checked;
      this.Entity.CreateViewOnly = this.cbCreateViewOnlyModal.Checked;
      this.Entity.CreateExcelExport = this.cbExcelExport.Checked;
      this.Entity.IsNonModalCRUDPage = this.cbNonModalCRUDPage.Checked;
      this.Entity.IsMasterDetailPage = this.cbMasterDetailPage.Checked;
      this.Entity.PagePermission = new PagePermissionInfo()
      {
        Host = this.cbHost.Checked,
        Tenant = this.cbTenant.Checked
      };
      this.Entity.CreateUserInterface = this.cbCreateUserInterface.Checked;
      if (this.Entity.CreateUserInterface)
        this.Entity.MenuPosition = this.cbMenuPosition.Text.Equals("Root") ? "main" : "admin";
      if (!this._mustSelectDbContext)
        return;
      this.Entity.DbContext = (DbContextDefinition) ((ComboboxItem) this.cmbDbContextSelect.SelectedItem).Value;
    }

    private bool CheckValidation()
    {
      if (EntityGeneratorForm.IsNullOrContainsWhiteSpace(this.txtRelativeNamespace.Text))
      {
        MsgBox.Warn("Enter a valid namespace! Namespace cannot be empty and shouldn't contain any whitespace.");
        this.txtRelativeNamespace.Focus();
        return false;
      }
      if (EntityGeneratorForm.IsNullOrContainsWhiteSpace(this.txtEntityName.Text))
      {
        MsgBox.Warn("Enter a valid entity name! Entity name cannot be empty and shouldn't contain any whitespace.");
        this.txtEntityName.Focus();
        return false;
      }
      if (EntityGeneratorForm.IsNullOrContainsWhiteSpace(this.txtTableName.Text))
      {
        MsgBox.Warn("Enter a valid table name! Table name cannot be empty and shouldn't contain any whitespace.");
        this.txtTableName.Focus();
        return false;
      }
      if (this.Entity.Properties.Count + this.Entity.NavigationProperties.Count < 1)
      {
        MsgBox.Warn("Enter at least one property!");
        this.SelectTabPanel(TabPanels.Properties);
        return false;
      }
      if (EntityGeneratorForm.IsNullOrContainsWhiteSpace(this.txtEntityNamePlural.Text))
      {
        MsgBox.Warn("Enter a valid plural entity name! Plural entity name cannot be empty and shouldn't contain any whitespace.");
        this.txtEntityNamePlural.Focus();
        return false;
      }
      if (this.txtEntityName.Text == this.txtRelativeNamespace.Text)
      {
        MsgBox.Warn("Entity name and the namespace cannot be the same!");
        this.txtEntityName.Focus();
        return false;
      }
      if (EntityGeneratorForm.IsCsharpKeyword(this.txtEntityName.Text))
      {
        MsgBox.Warn("\"" + this.txtEntityName.Text + "\" is a C# keyword and can not be used as an entity name!");
        this.txtEntityName.Focus();
        return false;
      }
      if (!this.cbMasterDetailPage.Checked || this.Entity.NavigationPropertyOneToManyTables.Count != 0)
        return true;
      MsgBox.Warn("You must select at least one navigation property (1-Many) table to create master-detail page!");
      this.SelectTabPanel(TabPanels.NavigationPropertiesOneToMany);
      return false;
    }

    private static bool IsCsharpKeyword(string entityNametxtText) => ((IEnumerable<string>) new string[78]
    {
      "abstract",
      "as",
      "base",
      "bool",
      "break",
      "byte",
      "case",
      "catch",
      "char",
      "checked",
      "class",
      "const",
      "continue",
      "decimal",
      "default",
      "delegate",
      "do",
      "double",
      "else",
      "enum",
      "event",
      "explicit",
      "extern",
      "false",
      "finally",
      "fixed",
      "float",
      "for",
      "foreach",
      "goto",
      "if",
      "implicit",
      "in",
      "int",
      "interface",
      "internal",
      "il",
      "lock",
      "long",
      "namespace",
      "new",
      "null",
      "object",
      "operator",
      "out",
      "override",
      "params",
      "private",
      "protected",
      "public",
      "readonly",
      "ref",
      "return",
      "sbyte",
      "sealed",
      "short",
      "sizeof",
      "stackalloc",
      "static",
      "string",
      "struct",
      "switch",
      "this",
      "throw",
      "tru",
      "try",
      "typeof",
      "uint",
      "ulong",
      "unchecked",
      "unsafe",
      "ushort",
      "using",
      "using static",
      "virtual",
      "void",
      "volatile",
      "while"
    }).Contains<string>(entityNametxtText.ToLowerInvariant());

    private void AddPropertyBox(Property property)
    {
      Panel propertyPanel = this.GetPropertyPanel(property);
      this.PropertyBoxes.Add(propertyPanel);
      this.PropertiesPanel.Controls.Add((Control) propertyPanel);
      this.SetCoordinatesOfProperties();
    }

    private void AddNavigationPropertyBox(Property property)
    {
      Panel propertyPanel = this.GetPropertyPanel(property);
      foreach (Control control in (ArrangedElementCollection) propertyPanel.Controls)
      {
        if (control.Name.Equals("btnEditProperty"))
        {
          control.Click -= new EventHandler(this.btnEditProperty_Click);
          control.Click += new EventHandler(this.EditNavigationProperty_click);
        }
      }
      foreach (Control control in (ArrangedElementCollection) propertyPanel.Controls)
      {
        if (control.Name.Equals("btnDeleteProperty"))
        {
          control.Click -= new EventHandler(this.btnDeleteProperty_click);
          control.Click += new EventHandler(this.DeleteNavigationProperty_click);
        }
      }
      this.NavigationPropertyBoxes.Add(propertyPanel);
      this.NavigationPropertiesPanel.Controls.Add((Control) propertyPanel);
      this.SetCoordinatesOfNavigationProperties();
    }

    private void SetCoordinatesOfProperties()
    {
      this.PropertiesPanel.VerticalScroll.Value = 0;
      for (int index = 0; index < this.PropertyBoxes.Count; ++index)
        this.PropertyBoxes[index].Location = new Point(19, 10 * (index + 1) + index * this.NewGroupBoxY);
    }

    private void SetCoordinatesOfNavigationProperties()
    {
      this.NavigationPropertiesPanel.VerticalScroll.Value = 0;
      for (int index = 0; index < this.NavigationPropertyBoxes.Count; ++index)
        this.NavigationPropertyBoxes[index].Location = new Point(19, 10 * (index + 1) + index * this.NewGroupBoxY);
    }

    private void btnGenerate_Click(object sender, EventArgs e)
    {
      try
      {
        EntityGeneratorForm.Logger.Debug((object) "Generate entity started.");
        if (!this.CheckValidation())
          EntityGeneratorForm.Logger.Debug((object) "Validation failed.");
        else if (!RadToolFileUpdater.CopyRadToolDlls(this.RootPath))
          EntityGeneratorForm.Logger.Debug((object) "Couldn't copy RAD Tool DLLs!");
        else if (!this.SetDuplicationNumbers())
          EntityGeneratorForm.Logger.Debug((object) "Couldn't set duplication numbers!");
        else if (!this.WriteEntityJsonFile())
          EntityGeneratorForm.Logger.Debug((object) "Couldn't write entity JSON file!");
        else if (!this.Generate(this.RootPath + "\\AspNetZeroRadTool"))
        {
          EntityGeneratorForm.Logger.Debug((object) "Couldn't generate!");
        }
        else
        {
          EntityGeneratorForm.Logger.Debug((object) "Entity successfully generated.");
          this.Close();
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Cannot generate entity!", ex);
      }
    }

    private void cbAddMigration_CheckedChanged(object sender, EventArgs e)
    {
      if (this.cbAddMigration.Checked)
      {
        this.cbUpdateDatabase.Enabled = true;
      }
      else
      {
        this.cbUpdateDatabase.Enabled = false;
        this.cbUpdateDatabase.Checked = false;
      }
    }

    private void MultiTenancyChecksChanged(object sender, EventArgs e)
    {
      if (this.cbTenant.Checked || this.cbHost.Checked || this.AllowNoneMultiTenancy)
        return;
      this.cbHost.Checked = true;
    }

    private void cbCreateUserInterface_Change(object sender, EventArgs e)
    {
      if (this.cbCreateUserInterface.Checked)
      {
        this.cbMenuPosition.Enabled = true;
        this.cbCreateViewOnlyModal.Enabled = true;
      }
      else
      {
        this.cbMenuPosition.Enabled = false;
        this.cbCreateViewOnlyModal.Enabled = false;
      }
    }

    private void btnDeleteProperty_click(object sender, EventArgs e)
    {
      string propertyName = EntityGeneratorForm.GetUniqueNameFromButtonName(sender.ToString());
      if (MsgBox.Question("Are you sure you want to delete the property \"{0}\"?", (object) propertyName) == DialogResult.No)
        return;
      Property property = this.Entity.Properties.Find((Predicate<Property>) (p => p.Name.Equals(propertyName)));
      this.Entity.Properties.Remove(property);
      if (this.Entity.Properties.All<Property>((Func<Property, bool>) (p => p.Name != property.Type)))
        this.Entity.EnumDefinitions.RemoveAll((Predicate<EnumDefinition>) (ed => ed.Name == property.Type));
      Panel panel = this.PropertyBoxes.Find((Predicate<Panel>) (p => p.Controls.Find("lblProperty", true)[0].Text.Contains(" " + propertyName + " ")));
      this.PropertyBoxes.Remove(panel);
      this.PropertiesPanel.Controls.Remove((Control) panel);
      this.SetCoordinatesOfProperties();
    }

    private void DeleteNavigationProperty_click(object sender, EventArgs e)
    {
      string propertyName = EntityGeneratorForm.GetUniqueNameFromButtonName(sender.ToString());
      if (MsgBox.Question("Are you sure you want to delete the navigation property \"{0}\"?", (object) propertyName) == DialogResult.No)
        return;
      this.Entity.NavigationProperties.Remove(this.Entity.NavigationProperties.Find((Predicate<NavigationProperty>) (np => np.PropertyName.Equals(propertyName))));
      Panel panel = this.NavigationPropertyBoxes.Find((Predicate<Panel>) (p => p.Controls.Find("lblProperty", true)[0].Text.Contains(" " + propertyName + " ")));
      this.NavigationPropertyBoxes.Remove(panel);
      this.NavigationPropertiesPanel.Controls.Remove((Control) panel);
      this.SetCoordinatesOfNavigationProperties();
    }

    private void btnEditProperty_Click(object sender, EventArgs e)
    {
      string propertyName = EntityGeneratorForm.GetUniqueNameFromButtonName(sender.ToString());
      Property property = this.Entity.Properties.Find((Predicate<Property>) (p => p.Name.Equals(propertyName)));
      int index1 = this.Entity.Properties.IndexOf(property);
      using (PropertyForm propertyForm = new PropertyForm(this.Entity, property, this.Entity.Properties.Select<Property, string>((Func<Property, string>) (p => p.Name)).ToList<string>()))
      {
        if (propertyForm.ShowDialog() != DialogResult.OK)
          return;
        this.Entity.Properties.Remove(property);
        this.Entity.Properties.Insert(index1, propertyForm.Property);
        Panel propertyPanel = this.GetPropertyPanel(property);
        Panel panel = this.PropertyBoxes.Find((Predicate<Panel>) (p => p.Controls.Find("lblProperty", true)[0].Text.Contains(" " + propertyName + " ")));
        int index2 = this.PropertyBoxes.IndexOf(panel);
        this.PropertyBoxes.Remove(panel);
        this.PropertyBoxes.Insert(index2, propertyPanel);
        this.PropertiesPanel.Controls.Remove((Control) panel);
        this.PropertiesPanel.Controls.Add((Control) propertyPanel);
        this.SetCoordinatesOfProperties();
        if (propertyForm.ImportedEnum == null || this.Entity.EnumDefinitions.Any<EnumDefinition>((Func<EnumDefinition, bool>) (ed => ed.Name.Equals(propertyForm.ImportedEnum.Name) && ed.Namespace.Equals(propertyForm.ImportedEnum.Namespace))))
          return;
        this.Entity.EnumDefinitions.Add(propertyForm.ImportedEnum);
      }
    }

    private void EditNavigationProperty_click(object sender, EventArgs e)
    {
      string propertyName = EntityGeneratorForm.GetUniqueNameFromButtonName(sender.ToString());
      NavigationProperty editEntity = this.Entity.NavigationProperties.Find((Predicate<NavigationProperty>) (p => p.PropertyName.Equals(propertyName)));
      int index1 = this.Entity.NavigationProperties.IndexOf(editEntity);
      using (ForeignKeySelectionForm keySelectionForm = new ForeignKeySelectionForm(editEntity, this.Entity.NavigationProperties))
      {
        if (keySelectionForm.ShowDialog() != DialogResult.OK)
          return;
        this.Entity.NavigationProperties.Remove(editEntity);
        this.Entity.NavigationProperties.Insert(index1, keySelectionForm.SelectedEntity);
        Panel propertyPanel = this.GetPropertyPanel(new Property()
        {
          Name = keySelectionForm.SelectedEntity.PropertyName,
          Type = keySelectionForm.SelectedEntity.ForeignEntityName
        });
        foreach (Control control in (ArrangedElementCollection) propertyPanel.Controls)
        {
          if (control.Name.Equals("btnEditProperty"))
          {
            control.Click -= new EventHandler(this.btnEditProperty_Click);
            control.Click += new EventHandler(this.EditNavigationProperty_click);
          }
        }
        foreach (Control control in (ArrangedElementCollection) propertyPanel.Controls)
        {
          if (control.Name.Equals("btnDeleteProperty"))
          {
            control.Click -= new EventHandler(this.btnDeleteProperty_click);
            control.Click += new EventHandler(this.DeleteNavigationProperty_click);
          }
        }
        Panel panel = this.NavigationPropertyBoxes.Find((Predicate<Panel>) (p => p.Controls.Find("lblProperty", true)[0].Text.Contains(" " + propertyName + " ")));
        int index2 = this.NavigationPropertyBoxes.IndexOf(panel);
        this.NavigationPropertyBoxes.Remove(panel);
        this.NavigationPropertyBoxes.Insert(index2, propertyPanel);
        this.NavigationPropertiesPanel.Controls.Remove((Control) panel);
        this.NavigationPropertiesPanel.Controls.Add((Control) propertyPanel);
        this.SetCoordinatesOfNavigationProperties();
      }
    }

    private void btnAddProperty_Click(object sender, EventArgs e)
    {
      using (PropertyForm propertyForm = new PropertyForm(this.Entity, new Property(), this.Entity.Properties.Select<Property, string>((Func<Property, string>) (p => p.Name)).ToList<string>()))
      {
        if (propertyForm.ShowDialog() != DialogResult.OK)
          return;
        this.Entity.Properties.Add(propertyForm.Property);
        this.AddPropertyBox(propertyForm.Property);
        if (propertyForm.ImportedEnum == null)
          return;
        this.Entity.EnumDefinitions.Add(propertyForm.ImportedEnum);
      }
    }

    private static string GetUniqueNameFromButtonName(string str)
    {
      int num = str.IndexOf('-');
      str = str.Substring(num + 1);
      int length = str.IndexOf('-');
      return str.Substring(0, length);
    }

    private static bool IsNullOrContainsWhiteSpace(string str) => string.IsNullOrWhiteSpace(str) || str.Contains(" ");

    private void txtEntityName_TextChanged(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.txtEntityName.Text))
      {
        this.txtEntityNamePlural.Text = "";
      }
      else
      {
        this.txtEntityNamePlural.Text = EntityGeneratorForm.PluralizeWord(this.txtEntityName.Text);
        this.txtTableName.Text = this.txtEntityNamePlural.Text;
      }
    }

    private static string PluralizeWord(string word)
    {
      if (word.EndsWith("y"))
        return word.Substring(0, word.Length - 1) + "ies";
      return word.EndsWith("x") || word.EndsWith("s") || word.EndsWith("i") ? word + "es" : word + "s";
    }

    private static string Upper(string word)
    {
      switch (word)
      {
        case "":
          return word;
        case null:
          return "";
        default:
          return word.Length == 1 ? word[0].ToString().ToUpper() : char.ToUpperInvariant(word[0]).ToString() + word.Substring(1);
      }
    }

    private void btnAddNavigationProperty_Click(object sender, EventArgs e)
    {
      try
      {
        using (ForeignKeySelectionForm keySelectionForm = new ForeignKeySelectionForm((NavigationProperty) null, (List<NavigationProperty>) null))
        {
          if (keySelectionForm.ShowDialog() != DialogResult.OK)
            return;
          this.Entity.NavigationProperties.Add(keySelectionForm.SelectedEntity);
          this.AddNavigationPropertyBox(new Property()
          {
            Name = keySelectionForm.SelectedEntity.PropertyName,
            Type = keySelectionForm.SelectedEntity.IdType
          });
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while adding navigation property!", ex);
      }
    }

    private bool SetDuplicationNumbers()
    {
      try
      {
        List<string> source = new List<string>();
        foreach (NavigationProperty navigationProperty1 in this.Entity.NavigationProperties)
        {
          NavigationProperty navigationProperty = navigationProperty1;
          int num = source.Count<string>((Func<string, bool>) (np => np.Equals(navigationProperty.ForeignEntityName)));
          navigationProperty.DuplicationNumber = num > 0 ? num + 1 : 0;
          source.Add(navigationProperty.ForeignEntityName);
        }
        return true;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error when setting duplication numbers!", ex);
        return false;
      }
    }

    private void SetLoadedEntity()
    {
      foreach (Property property in this.Entity.Properties)
        this.AddPropertyBox(property);
      foreach (NavigationProperty navigationProperty in this.Entity.NavigationProperties)
        this.AddNavigationPropertyBox(new Property()
        {
          Name = navigationProperty.PropertyName,
          Type = navigationProperty.IdType
        });
      this.cbCreateUserInterface.Checked = this.Entity.CreateUserInterface;
      this.cbCreateViewOnlyModal.Checked = this.Entity.CreateViewOnly;
      this.cbNonModalCRUDPage.Checked = this.Entity.IsNonModalCRUDPage;
      this.cbMasterDetailPage.Checked = this.Entity.IsMasterDetailPage;
      this.cbExcelExport.Checked = this.Entity.CreateExcelExport;
      this.cbMenuPosition.Text = this.Entity.MenuPosition.Equals("main") ? "Root" : "Administration";
      this.cbHost.Checked = this.Entity.PagePermission.Host;
      this.cbTenant.Checked = this.Entity.PagePermission.Tenant;
      this.cbUpdateDatabase.Checked = this.Entity.UpdateDatabase;
      this.cbAddMigration.Checked = this.Entity.AutoMigration;
      this.cbEntityHistory.Checked = this.Entity.EntityHistory;
      this.txtRelativeNamespace.Text = this.Entity.RelativeNamespace;
      this.txtEntityName.Text = this.Entity.EntityName;
      this.txtEntityNamePlural.Text = this.Entity.EntityNamePlural;
      this.txtTableName.Text = this.Entity.TableName;
      this.cmbBaseClass.Text = this.Entity.BaseClass;
      this.cmbPrimaryKey.Text = this.Entity.PrimaryKeyType;
      if (this.Entity.DbContext != null)
      {
        if (this.cmbDbContextSelect.Items.Count == 0)
          this.PopulateDbContextsCombobox();
        this.cmbDbContextSelect.SelectedIndex = this._allDbContexts.Select<DbContextDefinition, string>((Func<DbContextDefinition, string>) (x => x.FullPath)).ToList<string>().IndexOf(this.Entity.DbContext.FullPath);
      }
      foreach (NavigationPropertyOneToManyTable propertyOneToManyTable in this.Entity.NavigationPropertyOneToManyTables)
      {
        if (!propertyOneToManyTable.IsDeleted)
          this.AddNavigationPropertyOneToManyTableBox(propertyOneToManyTable.EntityJson, propertyOneToManyTable.ForeignPropertyName);
      }
    }

    private void btnEntityInformation_Click(object sender, EventArgs e) => this.SelectTabPanel(TabPanels.EntityInformation);

    private void btnProperties_Click(object sender, EventArgs e) => this.SelectTabPanel(TabPanels.Properties);

    private void btnNavigationProperties_Click(object sender, EventArgs e) => this.SelectTabPanel(TabPanels.NavigationProperties);

    private void btnChildEntitiesTab_Click(object sender, EventArgs e) => this.SelectTabPanel(TabPanels.NavigationPropertiesOneToMany);

    private static int? GetIndexFromTag(Control btn)
    {
      if ((string) btn.Tag == null)
        return new int?();
      int result;
      return int.TryParse(btn.Tag.ToString(), out result) ? new int?(result) : new int?();
    }

    private void SelectTabPanel(TabPanels selectedTabPanel)
    {
      int num1 = (int) selectedTabPanel;
      this.tabContainer.SelectedIndex = num1;
      foreach (Control control in (ArrangedElementCollection) this.pnlTabBtns.Controls)
      {
        if (control is Button ctrl1)
        {
          int? indexFromTag = EntityGeneratorForm.GetIndexFromTag((Control) ctrl1);
          int num2 = num1;
          if (indexFromTag.GetValueOrDefault() == num2 & indexFromTag.HasValue)
          {
            ctrl1.ForeColor = Color.White;
            ctrl1.SetFontStyleAsBold();
          }
          else
          {
            ctrl1.ForeColor = Color.Silver;
            ctrl1.SetFontStyleAsRegular();
          }
        }
      }
      foreach (Control control in (ArrangedElementCollection) this.pnlTabBtns.Controls)
      {
        if (control is Panel panel2)
        {
          int? indexFromTag = EntityGeneratorForm.GetIndexFromTag((Control) panel2);
          Panel panel = panel2;
          int? nullable = indexFromTag;
          int num3 = num1;
          int num4 = nullable.GetValueOrDefault() == num3 & nullable.HasValue ? 1 : 0;
          panel.Visible = num4 != 0;
        }
      }
    }

    private void txtRelativeNamespace_TextChanged(object sender, EventArgs e)
    {
      this.txtRelativeNamespace.Text = this.txtRelativeNamespace.Text.Trim();
      this.namespace2lbl.Text = string.IsNullOrEmpty(this.txtRelativeNamespace.Text) ? this.Entity.Namespace : string.Format("{0}.{1}", (object) this.Entity.Namespace, (object) this.txtRelativeNamespace.Text);
    }

    private void btnCancel_Click(object sender, EventArgs e) => this.Close();

    private void PopulateDbContextsCombobox()
    {
      this._allDbContexts = DbContextProvider.GetAllDbContexts();
      if (this._allDbContexts.Count < 2)
      {
        this._mustSelectDbContext = false;
        this.cmbDbContextSelect.Visible = false;
      }
      else
      {
        this._mustSelectDbContext = true;
        foreach (DbContextDefinition allDbContext in this._allDbContexts)
          this.cmbDbContextSelect.Items.Add((object) new ComboboxItem()
          {
            Text = (allDbContext.Name ?? ""),
            Value = (object) allDbContext
          });
        this.cmbDbContextSelect.SelectedIndex = 0;
        this.cmbDbContextSelect.Visible = true;
      }
    }

    private void AddNavigationPropertyOneToManyTableBox(string tableName, string propertyName)
    {
      Panel propertyPanel = this.GetPropertyPanel(new Property()
      {
        Name = tableName,
        Type = propertyName
      });
      foreach (Control control in (ArrangedElementCollection) propertyPanel.Controls)
      {
        if (control.Name.Equals("btnEditProperty"))
        {
          control.Click -= new EventHandler(this.btnEditProperty_Click);
          control.Click += new EventHandler(this.EditNavigationPropertyOneToManyTable_click);
        }
      }
      foreach (Control control in (ArrangedElementCollection) propertyPanel.Controls)
      {
        if (control.Name.Equals("btnDeleteProperty"))
        {
          control.Click -= new EventHandler(this.btnDeleteProperty_click);
          control.Click += new EventHandler(this.RemoveNavigationPropertyOneToManyTable_click);
        }
      }
      this.NavigationPropertyOneToManyTableBoxes.Add(propertyPanel);
      this.NavigationPropertyOneToManyTablesPanel.Controls.Add((Control) propertyPanel);
      this.SetCoordinatesOfNavigationPropertiesOneToMany();
    }

    private void EditNavigationPropertyOneToManyTable_click(object sender, EventArgs e)
    {
      string entityJson = EntityGeneratorForm.GetUniqueNameFromButtonName(sender.ToString());
      NavigationPropertyOneToManyTable editTable = this.Entity.NavigationPropertyOneToManyTables.Find((Predicate<NavigationPropertyOneToManyTable>) (p => p.EntityJson.Equals(entityJson)));
      int index = this.Entity.NavigationPropertyOneToManyTables.IndexOf(editTable);
      using (TableSelectionForm tableSelectionForm = new TableSelectionForm(EntityGeneratorForm.Upper(this.txtEntityName.Text), this.Entity, editTable))
      {
        if (tableSelectionForm.ShowDialog() != DialogResult.OK)
          return;
        editTable.IsDeleted = true;
        this.Entity.NavigationPropertyOneToManyTables.Insert(index, tableSelectionForm.SelectedTable);
        this.RemoveNavigationPropertyOneToManyTableBox(tableSelectionForm.SelectedTable.EntityJson);
        this.AddNavigationPropertyOneToManyTableBox(tableSelectionForm.SelectedTable.EntityJson, tableSelectionForm.SelectedTable.ForeignPropertyName);
      }
    }

    private void RemoveNavigationPropertyOneToManyTable_click(object sender, EventArgs e)
    {
      string tableName = EntityGeneratorForm.GetUniqueNameFromButtonName(sender.ToString());
      if (MsgBox.Question("Are you sure you want to remove table \"{0}\"?", (object) tableName) == DialogResult.No)
        return;
      this.Entity.NavigationPropertyOneToManyTables.Find((Predicate<NavigationPropertyOneToManyTable>) (np => np.EntityJson.Equals(tableName))).IsDeleted = true;
      this.RemoveNavigationPropertyOneToManyTableBox(tableName);
    }

    private void RemoveNavigationPropertyOneToManyTableBox(string entityJson)
    {
      Panel panel = this.NavigationPropertyOneToManyTableBoxes.Find((Predicate<Panel>) (p => p.Controls.Find("lblProperty", true)[0].Text.Contains(" " + entityJson + " ")));
      this.NavigationPropertyOneToManyTableBoxes.Remove(panel);
      this.NavigationPropertyOneToManyTablesPanel.Controls.Remove((Control) panel);
      this.SetCoordinatesOfNavigationPropertiesOneToMany();
    }

    private void SetCoordinatesOfNavigationPropertiesOneToMany()
    {
      this.NavigationPropertyOneToManyTablesPanel.VerticalScroll.Value = 0;
      for (int index = 0; index < this.NavigationPropertyOneToManyTableBoxes.Count; ++index)
        this.NavigationPropertyOneToManyTableBoxes[index].Location = new Point(19, 10 * (index + 1) + index * this.NewGroupBoxY);
    }

    private void cbMasterDetailPage_CheckedChanged(object sender, EventArgs e)
    {
      this.btnNavigationPropertyOneToManyTab.Visible = this.cbMasterDetailPage.Checked;
      if (this.cbMasterDetailPage.Checked)
      {
        this.pnlTabBtns.Location = new Point(60, 0);
        this.cbNonModalCRUDPage.Checked = false;
      }
      else
      {
        this.pnlTabBtns.Location = new Point(150, 0);
        this.Entity.NavigationPropertyOneToManyTables = new List<NavigationPropertyOneToManyTable>();
        foreach (Control oneToManyTableBox in this.NavigationPropertyOneToManyTableBoxes)
          this.NavigationPropertyOneToManyTablesPanel.Controls.Remove(oneToManyTableBox);
        this.NavigationPropertyOneToManyTableBoxes = new List<Panel>();
      }
    }

    private void btnAddNavigationPropertyOneToManyTable_Click(object sender, EventArgs e)
    {
      try
      {
        using (TableSelectionForm tableSelectionForm = new TableSelectionForm(EntityGeneratorForm.Upper(this.txtEntityName.Text), this.Entity))
        {
          if (tableSelectionForm.ShowDialog() != DialogResult.OK)
            return;
          this.Entity.NavigationPropertyOneToManyTables.Add(tableSelectionForm.SelectedTable);
          this.AddNavigationPropertyOneToManyTableBox(tableSelectionForm.SelectedTable.EntityJson, tableSelectionForm.SelectedTable.ForeignPropertyName);
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error occured while adding navigation property!", ex);
      }
    }

    private void cbNonModalCRUDPage_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.cbNonModalCRUDPage.Checked)
        return;
      this.cbMasterDetailPage.Checked = false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    public Panel GetPropertyPanel(Property property)
    {
      this.NewGroupBoxX = (int) (780.0 / 917.0 * (double) this.tabContainer.Size.Width);
      this.NewGroupBoxY = (int) (19.0 / 238.0 * (double) this.tabContainer.Size.Height);
      int width = (int) (1.0 / 13.0 * (double) this.NewGroupBoxX);
      int height = (int) (12.0 / 19.0 * (double) this.NewGroupBoxY);
      Panel panel1 = new Panel();
      Panel panel2 = new Panel();
      Button button1 = new Button();
      Button button2 = new Button();
      Label label1 = new Label();
      Label label2 = new Label();
      panel1.Controls.Add((Control) panel2);
      panel1.Controls.Add((Control) button1);
      panel1.Controls.Add((Control) button2);
      panel1.Controls.Add((Control) label1);
      panel1.BackColor = Color.FromArgb(38, 38, 42);
      panel1.Location = new Point(19, 4);
      panel1.Name = "newGroupBox_" + property.Name;
      panel1.Size = new Size(this.NewGroupBoxX, this.NewGroupBoxY);
      panel1.TabStop = false;
      button2.Location = new Point(this.NewGroupBoxX - width * 2 - 20, 6);
      button2.Name = "btnEditProperty";
      button2.Size = new Size(width, height);
      button2.TabIndex = 6;
      button2.Text = "   Edit         -" + property.Name + "-";
      button2.UseVisualStyleBackColor = true;
      button2.Click += new EventHandler(this.btnEditProperty_Click);
      button2.Font = new Font("Segoe UI", 9f);
      button2.ForeColor = Color.White;
      button2.BackColor = Color.FromArgb(38, 38, 42);
      button2.FlatAppearance.BorderColor = Color.White;
      button2.FlatStyle = FlatStyle.Flat;
      button2.Cursor = Cursors.Hand;
      Button button3 = button1;
      Point location = button2.Location;
      int x = location.X + button2.Size.Width + 10;
      location = button2.Location;
      int y = location.Y;
      Point point = new Point(x, y);
      button3.Location = point;
      button1.Name = "btnDeleteProperty";
      button1.TabIndex = 7;
      button1.Text = " Delete       -" + property.Name + "-";
      button1.UseVisualStyleBackColor = true;
      button1.Click += new EventHandler(this.btnDeleteProperty_click);
      button1.Size = button2.Size;
      button1.Font = button2.Font;
      button1.ForeColor = button2.ForeColor;
      button1.FlatAppearance.BorderColor = button2.FlatAppearance.BorderColor;
      button1.FlatStyle = button2.FlatStyle;
      button1.BackColor = button2.BackColor;
      button1.Cursor = Cursors.Hand;
      label2.AutoSize = true;
      label2.Location = new Point(28, 9);
      label2.Name = "lblType";
      label2.Font = new Font("Segoe UI", 10f);
      label2.ForeColor = Color.White;
      label2.Size = new Size(20, 30);
      label2.TabIndex = 11;
      label2.Text = property.Type;
      panel2.Controls.Add((Control) label2);
      panel2.BackColor = Color.FromArgb(80, 80, 80);
      panel2.Location = new Point(0, 0);
      panel2.Name = "newGroupBox_" + property.Name;
      panel2.Size = new Size(200, this.NewGroupBoxY);
      label1.AutoSize = true;
      label1.Location = new Point(panel2.Size.Width + 18, 9);
      label1.Name = "lblProperty";
      label1.Font = new Font("Segoe UI", 10f);
      label1.ForeColor = Color.White;
      label1.TabIndex = 2;
      label1.Text = " " + property.Name + " ";
      return panel1;
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (EntityGeneratorForm));
      this.tabContainer = new TabControl();
      this.EntityInfoTab = new TabPage();
      this.panel8 = new Panel();
      this.label11 = new Label();
      this.pnlUserInterface = new Panel();
      this.cbMasterDetailPage = new CheckBox();
      this.cbNonModalCRUDPage = new CheckBox();
      this.cbMenuPosition = new ComboBox();
      this.cbExcelExport = new CheckBox();
      this.label5 = new Label();
      this.cbCreateViewOnlyModal = new CheckBox();
      this.cbCreateUserInterface = new CheckBox();
      this.label10 = new Label();
      this.pnlMultiTenancy = new Panel();
      this.cbTenant = new CheckBox();
      this.cbHost = new CheckBox();
      this.label9 = new Label();
      this.pnlAuditing = new Panel();
      this.EntityHistoryDisabledLabel = new Label();
      this.cbEntityHistory = new CheckBox();
      this.label8 = new Label();
      this.pnlDatabaseMigrations = new Panel();
      this.cmbDbContextSelect = new ComboBox();
      this.cbUpdateDatabase = new CheckBox();
      this.cbAddMigration = new CheckBox();
      this.label7 = new Label();
      this.pnlEntityInfo = new Panel();
      this.txtEntityNamePlural = new TextBox();
      this.Namespacelbl = new Label();
      this.label6 = new Label();
      this.label2 = new Label();
      this.namespace2lbl = new Label();
      this.txtTableName = new TextBox();
      this.txtRelativeNamespace = new TextBox();
      this.label3 = new Label();
      this.cmbPrimaryKey = new ComboBox();
      this.label1 = new Label();
      this.cmbBaseClass = new ComboBox();
      this.label4 = new Label();
      this.txtEntityName = new TextBox();
      this.tabPage2 = new TabPage();
      this.panel12 = new Panel();
      this.label13 = new Label();
      this.label12 = new Label();
      this.btnAddProperty = new Button();
      this.PropertiesPanel = new Panel();
      this.NavigationTab = new TabPage();
      this.panel13 = new Panel();
      this.btnAddNavigationProperty = new Button();
      this.label14 = new Label();
      this.label15 = new Label();
      this.NavigationPropertiesPanel = new Panel();
      this.NavigationPropertyOneToManyTab = new TabPage();
      this.panel2 = new Panel();
      this.label16 = new Label();
      this.btnAddNavigationPropertyOneToManyTable = new Button();
      this.label17 = new Label();
      this.NavigationPropertyOneToManyTablesPanel = new Panel();
      this.btnGenerate = new Button();
      this.panelTop = new Panel();
      this.pnlTabBtns = new Panel();
      this.panel1 = new Panel();
      this.btnProperties = new Button();
      this.btnNavigationPropertyOneToManyTab = new Button();
      this.btnEntityInformation = new Button();
      this.panel11 = new Panel();
      this.btnNavigationProperties = new Button();
      this.panel10 = new Panel();
      this.panel9 = new Panel();
      this.pnlBottom = new Panel();
      this.btnCancel = new Button();
      this.panel3 = new Panel();
      this.tabContainer.SuspendLayout();
      this.EntityInfoTab.SuspendLayout();
      this.pnlUserInterface.SuspendLayout();
      this.pnlMultiTenancy.SuspendLayout();
      this.pnlAuditing.SuspendLayout();
      this.pnlDatabaseMigrations.SuspendLayout();
      this.pnlEntityInfo.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.panel12.SuspendLayout();
      this.NavigationTab.SuspendLayout();
      this.panel13.SuspendLayout();
      this.NavigationPropertyOneToManyTab.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panelTop.SuspendLayout();
      this.pnlTabBtns.SuspendLayout();
      this.pnlBottom.SuspendLayout();
      this.panel3.SuspendLayout();
      this.SuspendLayout();
      this.tabContainer.Controls.Add((Control) this.EntityInfoTab);
      this.tabContainer.Controls.Add((Control) this.tabPage2);
      this.tabContainer.Controls.Add((Control) this.NavigationTab);
      this.tabContainer.Controls.Add((Control) this.NavigationPropertyOneToManyTab);
      this.tabContainer.ImeMode = ImeMode.NoControl;
      this.tabContainer.Location = new Point(-8, -23);
      this.tabContainer.Margin = new Padding(0);
      this.tabContainer.Multiline = true;
      this.tabContainer.Name = "tabContainer";
      this.tabContainer.SelectedIndex = 0;
      this.tabContainer.Size = new Size(917, 476);
      this.tabContainer.TabIndex = 0;
      this.EntityInfoTab.BackColor = Color.FromArgb(38, 38, 42);
      this.EntityInfoTab.Controls.Add((Control) this.panel8);
      this.EntityInfoTab.Controls.Add((Control) this.label11);
      this.EntityInfoTab.Controls.Add((Control) this.pnlUserInterface);
      this.EntityInfoTab.Controls.Add((Control) this.label10);
      this.EntityInfoTab.Controls.Add((Control) this.pnlMultiTenancy);
      this.EntityInfoTab.Controls.Add((Control) this.label9);
      this.EntityInfoTab.Controls.Add((Control) this.pnlAuditing);
      this.EntityInfoTab.Controls.Add((Control) this.label8);
      this.EntityInfoTab.Controls.Add((Control) this.pnlDatabaseMigrations);
      this.EntityInfoTab.Controls.Add((Control) this.label7);
      this.EntityInfoTab.Controls.Add((Control) this.pnlEntityInfo);
      this.EntityInfoTab.Location = new Point(4, 22);
      this.EntityInfoTab.Margin = new Padding(0);
      this.EntityInfoTab.Name = "EntityInfoTab";
      this.EntityInfoTab.Padding = new Padding(3);
      this.EntityInfoTab.Size = new Size(909, 450);
      this.EntityInfoTab.TabIndex = 0;
      this.EntityInfoTab.Text = "Entity Information";
      this.panel8.Location = new Point(680, 42);
      this.panel8.Name = "panel8";
      this.panel8.Size = new Size(3, 140);
      this.panel8.TabIndex = 14;
      this.label11.AutoSize = true;
      this.label11.Font = new Font("Segoe UI", 11f);
      this.label11.ForeColor = Color.White;
      this.label11.Location = new Point(442, 19);
      this.label11.Name = "label11";
      this.label11.Size = new Size(100, 20);
      this.label11.TabIndex = 25;
      this.label11.Text = "User Interface";
      this.pnlUserInterface.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlUserInterface.BorderStyle = BorderStyle.FixedSingle;
      this.pnlUserInterface.Controls.Add((Control) this.cbMasterDetailPage);
      this.pnlUserInterface.Controls.Add((Control) this.cbNonModalCRUDPage);
      this.pnlUserInterface.Controls.Add((Control) this.cbMenuPosition);
      this.pnlUserInterface.Controls.Add((Control) this.cbExcelExport);
      this.pnlUserInterface.Controls.Add((Control) this.label5);
      this.pnlUserInterface.Controls.Add((Control) this.cbCreateViewOnlyModal);
      this.pnlUserInterface.Controls.Add((Control) this.cbCreateUserInterface);
      this.pnlUserInterface.Location = new Point(446, 43);
      this.pnlUserInterface.Name = "pnlUserInterface";
      this.pnlUserInterface.Size = new Size(423, 148);
      this.pnlUserInterface.TabIndex = 24;
      this.cbMasterDetailPage.AutoSize = true;
      this.cbMasterDetailPage.Cursor = Cursors.Hand;
      this.cbMasterDetailPage.Font = new Font("Segoe UI", 9.75f);
      this.cbMasterDetailPage.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbMasterDetailPage.Location = new Point(20, 117);
      this.cbMasterDetailPage.Name = "cbMasterDetailPage";
      this.cbMasterDetailPage.Size = new Size(181, 21);
      this.cbMasterDetailPage.TabIndex = 14;
      this.cbMasterDetailPage.Text = "Create Master-Detail Page";
      this.cbMasterDetailPage.TextAlign = ContentAlignment.MiddleCenter;
      this.cbMasterDetailPage.UseVisualStyleBackColor = true;
      this.cbMasterDetailPage.CheckedChanged += new EventHandler(this.cbMasterDetailPage_CheckedChanged);
      this.cbNonModalCRUDPage.AutoSize = true;
      this.cbNonModalCRUDPage.Cursor = Cursors.Hand;
      this.cbNonModalCRUDPage.Font = new Font("Segoe UI", 9.75f);
      this.cbNonModalCRUDPage.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbNonModalCRUDPage.Location = new Point(20, 90);
      this.cbNonModalCRUDPage.Name = "cbNonModalCRUDPage";
      this.cbNonModalCRUDPage.Size = new Size(207, 21);
      this.cbNonModalCRUDPage.TabIndex = 13;
      this.cbNonModalCRUDPage.Text = "Create Non-modal CRUD Page";
      this.cbNonModalCRUDPage.TextAlign = ContentAlignment.MiddleCenter;
      this.cbNonModalCRUDPage.UseVisualStyleBackColor = true;
      this.cbNonModalCRUDPage.CheckedChanged += new EventHandler(this.cbNonModalCRUDPage_CheckedChanged);
      this.cbMenuPosition.BackColor = Color.FromArgb(224, 224, 224);
      this.cbMenuPosition.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cbMenuPosition.FlatStyle = FlatStyle.Flat;
      this.cbMenuPosition.Font = new Font("Segoe UI", 9.75f);
      this.cbMenuPosition.ForeColor = Color.FromArgb(64, 64, 64);
      this.cbMenuPosition.FormattingEnabled = true;
      this.cbMenuPosition.Location = new Point(248, 53);
      this.cbMenuPosition.Name = "cbMenuPosition";
      this.cbMenuPosition.Size = new Size(155, 25);
      this.cbMenuPosition.TabIndex = 12;
      this.cbExcelExport.AutoSize = true;
      this.cbExcelExport.Checked = true;
      this.cbExcelExport.CheckState = CheckState.Checked;
      this.cbExcelExport.Cursor = Cursors.Hand;
      this.cbExcelExport.Font = new Font("Segoe UI", 9.75f);
      this.cbExcelExport.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbExcelExport.Location = new Point(20, 63);
      this.cbExcelExport.Name = "cbExcelExport";
      this.cbExcelExport.Size = new Size(181, 21);
      this.cbExcelExport.TabIndex = 11;
      this.cbExcelExport.Text = "Create Excel Export Button";
      this.cbExcelExport.TextAlign = ContentAlignment.MiddleCenter;
      this.cbExcelExport.UseVisualStyleBackColor = true;
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Segoe UI", 9.75f);
      this.label5.ForeColor = Color.FromArgb(221, 221, 221);
      this.label5.Location = new Point(248, 31);
      this.label5.Name = "label5";
      this.label5.Size = new Size(91, 17);
      this.label5.TabIndex = 2;
      this.label5.Text = "Menu Position";
      this.cbCreateViewOnlyModal.AutoSize = true;
      this.cbCreateViewOnlyModal.Checked = true;
      this.cbCreateViewOnlyModal.CheckState = CheckState.Checked;
      this.cbCreateViewOnlyModal.Cursor = Cursors.Hand;
      this.cbCreateViewOnlyModal.Font = new Font("Segoe UI", 9.75f);
      this.cbCreateViewOnlyModal.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbCreateViewOnlyModal.Location = new Point(20, 36);
      this.cbCreateViewOnlyModal.Name = "cbCreateViewOnlyModal";
      this.cbCreateViewOnlyModal.Size = new Size(126, 21);
      this.cbCreateViewOnlyModal.TabIndex = 10;
      this.cbCreateViewOnlyModal.Text = "Create View Only";
      this.cbCreateViewOnlyModal.TextAlign = ContentAlignment.MiddleCenter;
      this.cbCreateViewOnlyModal.UseVisualStyleBackColor = true;
      this.cbCreateUserInterface.AutoSize = true;
      this.cbCreateUserInterface.Checked = true;
      this.cbCreateUserInterface.CheckState = CheckState.Checked;
      this.cbCreateUserInterface.Cursor = Cursors.Hand;
      this.cbCreateUserInterface.Font = new Font("Segoe UI", 9.75f);
      this.cbCreateUserInterface.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbCreateUserInterface.Location = new Point(20, 11);
      this.cbCreateUserInterface.Name = "cbCreateUserInterface";
      this.cbCreateUserInterface.Size = new Size(150, 21);
      this.cbCreateUserInterface.TabIndex = 9;
      this.cbCreateUserInterface.Text = "Create User Interface";
      this.cbCreateUserInterface.TextAlign = ContentAlignment.MiddleCenter;
      this.cbCreateUserInterface.UseVisualStyleBackColor = true;
      this.cbCreateUserInterface.CheckedChanged += new EventHandler(this.cbCreateUserInterface_Change);
      this.label10.AutoSize = true;
      this.label10.Font = new Font("Segoe UI", 11f);
      this.label10.ForeColor = Color.White;
      this.label10.Location = new Point(442, 202);
      this.label10.Name = "label10";
      this.label10.Size = new Size(100, 20);
      this.label10.TabIndex = 23;
      this.label10.Text = "Multi Tenancy";
      this.pnlMultiTenancy.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlMultiTenancy.BorderStyle = BorderStyle.FixedSingle;
      this.pnlMultiTenancy.Controls.Add((Control) this.cbTenant);
      this.pnlMultiTenancy.Controls.Add((Control) this.cbHost);
      this.pnlMultiTenancy.Location = new Point(446, 226);
      this.pnlMultiTenancy.Name = "pnlMultiTenancy";
      this.pnlMultiTenancy.Size = new Size(423, 82);
      this.pnlMultiTenancy.TabIndex = 22;
      this.cbTenant.AutoSize = true;
      this.cbTenant.Checked = true;
      this.cbTenant.CheckState = CheckState.Checked;
      this.cbTenant.Cursor = Cursors.Hand;
      this.cbTenant.Font = new Font("Segoe UI", 9.75f);
      this.cbTenant.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbTenant.Location = new Point(20, 45);
      this.cbTenant.Name = "cbTenant";
      this.cbTenant.Size = new Size(65, 21);
      this.cbTenant.TabIndex = 14;
      this.cbTenant.Text = "Tenant";
      this.cbTenant.UseVisualStyleBackColor = true;
      this.cbTenant.CheckedChanged += new EventHandler(this.MultiTenancyChecksChanged);
      this.cbHost.AutoSize = true;
      this.cbHost.Checked = true;
      this.cbHost.CheckState = CheckState.Checked;
      this.cbHost.Cursor = Cursors.Hand;
      this.cbHost.Font = new Font("Segoe UI", 9.75f);
      this.cbHost.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbHost.Location = new Point(20, 12);
      this.cbHost.Name = "cbHost";
      this.cbHost.Size = new Size(54, 21);
      this.cbHost.TabIndex = 13;
      this.cbHost.Text = "Host";
      this.cbHost.UseVisualStyleBackColor = true;
      this.cbHost.CheckedChanged += new EventHandler(this.MultiTenancyChecksChanged);
      this.label9.AutoSize = true;
      this.label9.Font = new Font("Segoe UI", 11f);
      this.label9.ForeColor = Color.White;
      this.label9.Location = new Point(442, 317);
      this.label9.Name = "label9";
      this.label9.Size = new Size(66, 20);
      this.label9.TabIndex = 21;
      this.label9.Text = "Auditing";
      this.pnlAuditing.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlAuditing.BorderStyle = BorderStyle.FixedSingle;
      this.pnlAuditing.Controls.Add((Control) this.EntityHistoryDisabledLabel);
      this.pnlAuditing.Controls.Add((Control) this.cbEntityHistory);
      this.pnlAuditing.Location = new Point(446, 341);
      this.pnlAuditing.Name = "pnlAuditing";
      this.pnlAuditing.Size = new Size(423, 70);
      this.pnlAuditing.TabIndex = 20;
      this.EntityHistoryDisabledLabel.AutoSize = true;
      this.EntityHistoryDisabledLabel.Font = new Font("Segoe UI", 9f, FontStyle.Italic);
      this.EntityHistoryDisabledLabel.ForeColor = Color.FromArgb(120, 120, 120);
      this.EntityHistoryDisabledLabel.Location = new Point(157, 24);
      this.EntityHistoryDisabledLabel.Name = "EntityHistoryDisabledLabel";
      this.EntityHistoryDisabledLabel.Size = new Size(267, 30);
      this.EntityHistoryDisabledLabel.TabIndex = 11;
      this.EntityHistoryDisabledLabel.Text = "This feature will not be active in your application, \r\nif entity history is disabled in your solution.";
      this.cbEntityHistory.AutoSize = true;
      this.cbEntityHistory.Cursor = Cursors.Hand;
      this.cbEntityHistory.Font = new Font("Segoe UI", 9.75f);
      this.cbEntityHistory.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbEntityHistory.Location = new Point(20, 25);
      this.cbEntityHistory.Name = "cbEntityHistory";
      this.cbEntityHistory.Size = new Size(137, 21);
      this.cbEntityHistory.TabIndex = 15;
      this.cbEntityHistory.Text = "Track Entity History";
      this.cbEntityHistory.UseVisualStyleBackColor = true;
      this.label8.AutoSize = true;
      this.label8.Font = new Font("Segoe UI", 11f);
      this.label8.ForeColor = Color.White;
      this.label8.Location = new Point(25, 317);
      this.label8.Name = "label8";
      this.label8.Size = new Size(147, 20);
      this.label8.TabIndex = 19;
      this.label8.Text = "Database Migrations";
      this.pnlDatabaseMigrations.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlDatabaseMigrations.BorderStyle = BorderStyle.FixedSingle;
      this.pnlDatabaseMigrations.Controls.Add((Control) this.cmbDbContextSelect);
      this.pnlDatabaseMigrations.Controls.Add((Control) this.cbUpdateDatabase);
      this.pnlDatabaseMigrations.Controls.Add((Control) this.cbAddMigration);
      this.pnlDatabaseMigrations.Location = new Point(30, 341);
      this.pnlDatabaseMigrations.Name = "pnlDatabaseMigrations";
      this.pnlDatabaseMigrations.Size = new Size(386, 70);
      this.pnlDatabaseMigrations.TabIndex = 18;
      this.cmbDbContextSelect.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbDbContextSelect.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbDbContextSelect.FlatStyle = FlatStyle.Flat;
      this.cmbDbContextSelect.Font = new Font("Segoe UI", 9.75f);
      this.cmbDbContextSelect.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbDbContextSelect.FormattingEnabled = true;
      this.cmbDbContextSelect.Location = new Point(153, 19);
      this.cmbDbContextSelect.Name = "cmbDbContextSelect";
      this.cmbDbContextSelect.Size = new Size(221, 25);
      this.cmbDbContextSelect.TabIndex = 9;
      this.cbUpdateDatabase.AutoSize = true;
      this.cbUpdateDatabase.Checked = true;
      this.cbUpdateDatabase.CheckState = CheckState.Checked;
      this.cbUpdateDatabase.Cursor = Cursors.Hand;
      this.cbUpdateDatabase.Font = new Font("Segoe UI", 9.75f);
      this.cbUpdateDatabase.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbUpdateDatabase.Location = new Point(18, 39);
      this.cbUpdateDatabase.Name = "cbUpdateDatabase";
      this.cbUpdateDatabase.Size = new Size(129, 21);
      this.cbUpdateDatabase.TabIndex = 8;
      this.cbUpdateDatabase.Text = "Update Database";
      this.cbUpdateDatabase.UseVisualStyleBackColor = true;
      this.cbAddMigration.AutoSize = true;
      this.cbAddMigration.Checked = true;
      this.cbAddMigration.CheckState = CheckState.Checked;
      this.cbAddMigration.Cursor = Cursors.Hand;
      this.cbAddMigration.Font = new Font("Segoe UI", 9.75f);
      this.cbAddMigration.ForeColor = Color.FromArgb(221, 221, 221);
      this.cbAddMigration.Location = new Point(18, 10);
      this.cbAddMigration.Name = "cbAddMigration";
      this.cbAddMigration.Size = new Size(112, 21);
      this.cbAddMigration.TabIndex = 7;
      this.cbAddMigration.Text = "Add Migration";
      this.cbAddMigration.UseVisualStyleBackColor = true;
      this.cbAddMigration.CheckedChanged += new EventHandler(this.cbAddMigration_CheckedChanged);
      this.label7.AutoSize = true;
      this.label7.Font = new Font("Segoe UI", 11f);
      this.label7.ForeColor = Color.White;
      this.label7.Location = new Point(26, 19);
      this.label7.Name = "label7";
      this.label7.Size = new Size(76, 20);
      this.label7.TabIndex = 17;
      this.label7.Text = "Entity Info";
      this.pnlEntityInfo.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlEntityInfo.BorderStyle = BorderStyle.FixedSingle;
      this.pnlEntityInfo.Controls.Add((Control) this.txtEntityNamePlural);
      this.pnlEntityInfo.Controls.Add((Control) this.Namespacelbl);
      this.pnlEntityInfo.Controls.Add((Control) this.label6);
      this.pnlEntityInfo.Controls.Add((Control) this.label2);
      this.pnlEntityInfo.Controls.Add((Control) this.namespace2lbl);
      this.pnlEntityInfo.Controls.Add((Control) this.txtTableName);
      this.pnlEntityInfo.Controls.Add((Control) this.txtRelativeNamespace);
      this.pnlEntityInfo.Controls.Add((Control) this.label3);
      this.pnlEntityInfo.Controls.Add((Control) this.cmbPrimaryKey);
      this.pnlEntityInfo.Controls.Add((Control) this.label1);
      this.pnlEntityInfo.Controls.Add((Control) this.cmbBaseClass);
      this.pnlEntityInfo.Controls.Add((Control) this.label4);
      this.pnlEntityInfo.Controls.Add((Control) this.txtEntityName);
      this.pnlEntityInfo.Location = new Point(30, 43);
      this.pnlEntityInfo.Name = "pnlEntityInfo";
      this.pnlEntityInfo.Size = new Size(386, 258);
      this.pnlEntityInfo.TabIndex = 16;
      this.txtEntityNamePlural.BackColor = Color.FromArgb(224, 224, 224);
      this.txtEntityNamePlural.BorderStyle = BorderStyle.FixedSingle;
      this.txtEntityNamePlural.Font = new Font("Segoe UI", 9.75f);
      this.txtEntityNamePlural.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtEntityNamePlural.Location = new Point(191, 96);
      this.txtEntityNamePlural.Name = "txtEntityNamePlural";
      this.txtEntityNamePlural.Size = new Size(169, 25);
      this.txtEntityNamePlural.TabIndex = 3;
      this.Namespacelbl.AutoSize = true;
      this.Namespacelbl.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.Namespacelbl.ForeColor = Color.FromArgb(221, 221, 221);
      this.Namespacelbl.Location = new Point(16, 20);
      this.Namespacelbl.Name = "Namespacelbl";
      this.Namespacelbl.Size = new Size(77, 17);
      this.Namespacelbl.TabIndex = 1;
      this.Namespacelbl.Text = "Namespace";
      this.label6.AutoSize = true;
      this.label6.Font = new Font("Segoe UI", 9.75f);
      this.label6.ForeColor = Color.FromArgb(221, 221, 221);
      this.label6.Location = new Point(16, 98);
      this.label6.Name = "label6";
      this.label6.Size = new Size(123, 17);
      this.label6.TabIndex = 13;
      this.label6.Text = "Entity Name (plural)";
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Segoe UI", 9.75f);
      this.label2.ForeColor = Color.FromArgb(221, 221, 221);
      this.label2.Location = new Point(16, 137);
      this.label2.Name = "label2";
      this.label2.Size = new Size(137, 17);
      this.label2.TabIndex = 5;
      this.label2.Text = "Database Table Name";
      this.namespace2lbl.BackColor = Color.FromArgb(83, 83, 84);
      this.namespace2lbl.Font = new Font("Segoe UI", 7.7f, FontStyle.Italic);
      this.namespace2lbl.ForeColor = Color.FromArgb(180, 180, 180);
      this.namespace2lbl.Location = new Point(16, 42);
      this.namespace2lbl.Name = "namespace2lbl";
      this.namespace2lbl.Size = new Size(343, 15);
      this.namespace2lbl.TabIndex = 11;
      this.namespace2lbl.Text = "MyCompany.AspNetZeroTemplate";
      this.txtTableName.BackColor = Color.FromArgb(224, 224, 224);
      this.txtTableName.BorderStyle = BorderStyle.FixedSingle;
      this.txtTableName.Font = new Font("Segoe UI", 9.75f);
      this.txtTableName.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtTableName.Location = new Point(191, 135);
      this.txtTableName.Name = "txtTableName";
      this.txtTableName.Size = new Size(169, 25);
      this.txtTableName.TabIndex = 4;
      this.txtRelativeNamespace.BackColor = Color.FromArgb(224, 224, 224);
      this.txtRelativeNamespace.BorderStyle = BorderStyle.FixedSingle;
      this.txtRelativeNamespace.Font = new Font("Segoe UI", 9.75f);
      this.txtRelativeNamespace.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtRelativeNamespace.Location = new Point(191, 18);
      this.txtRelativeNamespace.Name = "txtRelativeNamespace";
      this.txtRelativeNamespace.Size = new Size(169, 25);
      this.txtRelativeNamespace.TabIndex = 1;
      this.txtRelativeNamespace.TextChanged += new EventHandler(this.txtRelativeNamespace_TextChanged);
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Segoe UI", 9.75f);
      this.label3.ForeColor = Color.FromArgb(221, 221, 221);
      this.label3.Location = new Point(16, 177);
      this.label3.Name = "label3";
      this.label3.Size = new Size(69, 17);
      this.label3.TabIndex = 7;
      this.label3.Text = "Base Class";
      this.cmbPrimaryKey.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbPrimaryKey.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbPrimaryKey.FlatStyle = FlatStyle.Flat;
      this.cmbPrimaryKey.Font = new Font("Segoe UI", 9.75f);
      this.cmbPrimaryKey.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbPrimaryKey.FormattingEnabled = true;
      this.cmbPrimaryKey.Location = new Point(191, 213);
      this.cmbPrimaryKey.Name = "cmbPrimaryKey";
      this.cmbPrimaryKey.Size = new Size(169, 25);
      this.cmbPrimaryKey.TabIndex = 6;
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Segoe UI", 9.75f);
      this.label1.ForeColor = Color.FromArgb(221, 221, 221);
      this.label1.Location = new Point(16, 61);
      this.label1.Name = "label1";
      this.label1.Size = new Size(78, 17);
      this.label1.TabIndex = 3;
      this.label1.Text = "Entity Name";
      this.cmbBaseClass.BackColor = Color.FromArgb(224, 224, 224);
      this.cmbBaseClass.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cmbBaseClass.FlatStyle = FlatStyle.Flat;
      this.cmbBaseClass.Font = new Font("Segoe UI", 9.75f);
      this.cmbBaseClass.ForeColor = Color.FromArgb(64, 64, 64);
      this.cmbBaseClass.FormattingEnabled = true;
      this.cmbBaseClass.Location = new Point(191, 174);
      this.cmbBaseClass.Name = "cmbBaseClass";
      this.cmbBaseClass.Size = new Size(169, 25);
      this.cmbBaseClass.TabIndex = 5;
      this.label4.AutoSize = true;
      this.label4.Font = new Font("Segoe UI", 9.75f);
      this.label4.ForeColor = Color.FromArgb(221, 221, 221);
      this.label4.Location = new Point(16, 216);
      this.label4.Name = "label4";
      this.label4.Size = new Size(77, 17);
      this.label4.TabIndex = 9;
      this.label4.Text = "Primary Key";
      this.txtEntityName.BackColor = Color.FromArgb(224, 224, 224);
      this.txtEntityName.BorderStyle = BorderStyle.FixedSingle;
      this.txtEntityName.Font = new Font("Segoe UI", 9.75f);
      this.txtEntityName.ForeColor = Color.FromArgb(64, 64, 64);
      this.txtEntityName.Location = new Point(190, 57);
      this.txtEntityName.Name = "txtEntityName";
      this.txtEntityName.Size = new Size(169, 25);
      this.txtEntityName.TabIndex = 2;
      this.txtEntityName.TextChanged += new EventHandler(this.txtEntityName_TextChanged);
      this.tabPage2.BackColor = Color.FromArgb(38, 38, 42);
      this.tabPage2.Controls.Add((Control) this.panel12);
      this.tabPage2.Controls.Add((Control) this.PropertiesPanel);
      this.tabPage2.Location = new Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new Padding(3);
      this.tabPage2.Size = new Size(909, 450);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Properties";
      this.panel12.BackColor = Color.FromArgb(70, 70, 70);
      this.panel12.Controls.Add((Control) this.label13);
      this.panel12.Controls.Add((Control) this.label12);
      this.panel12.Controls.Add((Control) this.btnAddProperty);
      this.panel12.Location = new Point(33, 24);
      this.panel12.Name = "panel12";
      this.panel12.Size = new Size(842, 58);
      this.panel12.TabIndex = 2;
      this.label13.AutoSize = true;
      this.label13.Font = new Font("Segoe UI", 11f);
      this.label13.ForeColor = Color.White;
      this.label13.Location = new Point(238, 18);
      this.label13.Name = "label13";
      this.label13.Size = new Size(109, 20);
      this.label13.TabIndex = 2;
      this.label13.Text = "Property Name";
      this.label12.AutoSize = true;
      this.label12.Font = new Font("Segoe UI", 11f);
      this.label12.ForeColor = Color.White;
      this.label12.Location = new Point(45, 18);
      this.label12.Name = "label12";
      this.label12.Size = new Size(100, 20);
      this.label12.TabIndex = 1;
      this.label12.Text = "Property Type";
      this.btnAddProperty.BackColor = Color.FromArgb(3, 184, 120);
      this.btnAddProperty.Cursor = Cursors.Hand;
      this.btnAddProperty.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnAddProperty.FlatStyle = FlatStyle.Flat;
      this.btnAddProperty.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
      this.btnAddProperty.ForeColor = Color.White;
      this.btnAddProperty.Location = new Point(629, 10);
      this.btnAddProperty.Name = "btnAddProperty";
      this.btnAddProperty.Size = new Size(194, 36);
      this.btnAddProperty.TabIndex = 16;
      this.btnAddProperty.Text = "ADD NEW PROPERTY";
      this.btnAddProperty.UseVisualStyleBackColor = false;
      this.btnAddProperty.Click += new EventHandler(this.btnAddProperty_Click);
      this.PropertiesPanel.AutoScroll = true;
      this.PropertiesPanel.BackColor = Color.FromArgb(50, 50, 50);
      this.PropertiesPanel.Location = new Point(33, 83);
      this.PropertiesPanel.Name = "PropertiesPanel";
      this.PropertiesPanel.Size = new Size(842, 305);
      this.PropertiesPanel.TabIndex = 1;
      this.NavigationTab.BackColor = Color.FromArgb(40, 40, 40);
      this.NavigationTab.Controls.Add((Control) this.panel13);
      this.NavigationTab.Controls.Add((Control) this.NavigationPropertiesPanel);
      this.NavigationTab.Location = new Point(4, 22);
      this.NavigationTab.Name = "NavigationTab";
      this.NavigationTab.Padding = new Padding(3);
      this.NavigationTab.Size = new Size(909, 450);
      this.NavigationTab.TabIndex = 3;
      this.NavigationTab.Text = "Navigation Properties";
      this.panel13.BackColor = Color.FromArgb(70, 70, 70);
      this.panel13.Controls.Add((Control) this.btnAddNavigationProperty);
      this.panel13.Controls.Add((Control) this.label14);
      this.panel13.Controls.Add((Control) this.label15);
      this.panel13.Location = new Point(33, 24);
      this.panel13.Name = "panel13";
      this.panel13.Size = new Size(842, 58);
      this.panel13.TabIndex = 3;
      this.btnAddNavigationProperty.BackColor = Color.FromArgb(3, 184, 120);
      this.btnAddNavigationProperty.BackgroundImageLayout = ImageLayout.None;
      this.btnAddNavigationProperty.Cursor = Cursors.Hand;
      this.btnAddNavigationProperty.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnAddNavigationProperty.FlatStyle = FlatStyle.Flat;
      this.btnAddNavigationProperty.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
      this.btnAddNavigationProperty.ForeColor = Color.White;
      this.btnAddNavigationProperty.Location = new Point(526, 10);
      this.btnAddNavigationProperty.Name = "btnAddNavigationProperty";
      this.btnAddNavigationProperty.Size = new Size(297, 36);
      this.btnAddNavigationProperty.TabIndex = 17;
      this.btnAddNavigationProperty.Text = "ADD NEW NAVIGATION PROPERTY";
      this.btnAddNavigationProperty.UseVisualStyleBackColor = false;
      this.btnAddNavigationProperty.Click += new EventHandler(this.btnAddNavigationProperty_Click);
      this.label14.AutoSize = true;
      this.label14.Font = new Font("Segoe UI", 11f);
      this.label14.ForeColor = Color.White;
      this.label14.Location = new Point(238, 18);
      this.label14.Name = "label14";
      this.label14.Size = new Size(109, 20);
      this.label14.TabIndex = 2;
      this.label14.Text = "Property Name";
      this.label15.AutoSize = true;
      this.label15.Font = new Font("Segoe UI", 11f);
      this.label15.ForeColor = Color.White;
      this.label15.Location = new Point(45, 18);
      this.label15.Name = "label15";
      this.label15.Size = new Size(100, 20);
      this.label15.TabIndex = 1;
      this.label15.Text = "Property Type";
      this.NavigationPropertiesPanel.AutoScroll = true;
      this.NavigationPropertiesPanel.BackColor = Color.FromArgb(50, 50, 50);
      this.NavigationPropertiesPanel.Location = new Point(33, 83);
      this.NavigationPropertiesPanel.Name = "NavigationPropertiesPanel";
      this.NavigationPropertiesPanel.Size = new Size(842, 305);
      this.NavigationPropertiesPanel.TabIndex = 1;
      this.NavigationPropertyOneToManyTab.BackColor = Color.FromArgb(38, 38, 42);
      this.NavigationPropertyOneToManyTab.Controls.Add((Control) this.panel2);
      this.NavigationPropertyOneToManyTab.Controls.Add((Control) this.NavigationPropertyOneToManyTablesPanel);
      this.NavigationPropertyOneToManyTab.Location = new Point(4, 22);
      this.NavigationPropertyOneToManyTab.Margin = new Padding(0);
      this.NavigationPropertyOneToManyTab.Name = "NavigationPropertyOneToManyTab";
      this.NavigationPropertyOneToManyTab.Padding = new Padding(3);
      this.NavigationPropertyOneToManyTab.Size = new Size(909, 450);
      this.NavigationPropertyOneToManyTab.TabIndex = 0;
      this.NavigationPropertyOneToManyTab.Text = "NavigationPropertyOneToManyTab";
      this.panel2.BackColor = Color.FromArgb(70, 70, 70);
      this.panel2.Controls.Add((Control) this.label16);
      this.panel2.Controls.Add((Control) this.btnAddNavigationPropertyOneToManyTable);
      this.panel2.Controls.Add((Control) this.label17);
      this.panel2.Location = new Point(33, 24);
      this.panel2.Name = "panel2";
      this.panel2.Size = new Size(842, 58);
      this.panel2.TabIndex = 5;
      this.label16.AutoSize = true;
      this.label16.Font = new Font("Segoe UI", 11f);
      this.label16.ForeColor = Color.White;
      this.label16.Location = new Point(262, 18);
      this.label16.Name = "label16";
      this.label16.Size = new Size(88, 20);
      this.label16.TabIndex = 18;
      this.label16.Text = "Table Name";
      this.btnAddNavigationPropertyOneToManyTable.BackColor = Color.FromArgb(3, 184, 120);
      this.btnAddNavigationPropertyOneToManyTable.BackgroundImageLayout = ImageLayout.None;
      this.btnAddNavigationPropertyOneToManyTable.Cursor = Cursors.Hand;
      this.btnAddNavigationPropertyOneToManyTable.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnAddNavigationPropertyOneToManyTable.FlatStyle = FlatStyle.Flat;
      this.btnAddNavigationPropertyOneToManyTable.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
      this.btnAddNavigationPropertyOneToManyTable.ForeColor = Color.White;
      this.btnAddNavigationPropertyOneToManyTable.Location = new Point(526, 10);
      this.btnAddNavigationPropertyOneToManyTable.Name = "btnAddNavigationPropertyOneToManyTable";
      this.btnAddNavigationPropertyOneToManyTable.Size = new Size(297, 36);
      this.btnAddNavigationPropertyOneToManyTable.TabIndex = 17;
      this.btnAddNavigationPropertyOneToManyTable.Text = "ADD TABLE";
      this.btnAddNavigationPropertyOneToManyTable.UseVisualStyleBackColor = false;
      this.btnAddNavigationPropertyOneToManyTable.Click += new EventHandler(this.btnAddNavigationPropertyOneToManyTable_Click);
      this.label17.AutoSize = true;
      this.label17.Font = new Font("Segoe UI", 11f);
      this.label17.ForeColor = Color.White;
      this.label17.Location = new Point(45, 18);
      this.label17.Name = "label17";
      this.label17.Size = new Size(109, 20);
      this.label17.TabIndex = 1;
      this.label17.Text = "Property Name";
      this.NavigationPropertyOneToManyTablesPanel.AutoScroll = true;
      this.NavigationPropertyOneToManyTablesPanel.BackColor = Color.FromArgb(50, 50, 50);
      this.NavigationPropertyOneToManyTablesPanel.Location = new Point(33, 83);
      this.NavigationPropertyOneToManyTablesPanel.Name = "NavigationPropertyOneToManyTablesPanel";
      this.NavigationPropertyOneToManyTablesPanel.Size = new Size(842, 305);
      this.NavigationPropertyOneToManyTablesPanel.TabIndex = 4;
      this.btnGenerate.BackColor = Color.FromArgb(3, 184, 120);
      this.btnGenerate.Cursor = Cursors.Hand;
      this.btnGenerate.FlatAppearance.BorderColor = Color.FromArgb(3, 184, 120);
      this.btnGenerate.FlatStyle = FlatStyle.Flat;
      this.btnGenerate.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
      this.btnGenerate.ForeColor = Color.White;
      this.btnGenerate.Location = new Point(295, 15);
      this.btnGenerate.Name = "btnGenerate";
      this.btnGenerate.Size = new Size(279, 47);
      this.btnGenerate.TabIndex = 18;
      this.btnGenerate.Text = "GENERATE";
      this.btnGenerate.UseVisualStyleBackColor = false;
      this.btnGenerate.Click += new EventHandler(this.btnGenerate_Click);
      this.panelTop.BackColor = Color.FromArgb(83, 83, 84);
      this.panelTop.Controls.Add((Control) this.pnlTabBtns);
      this.panelTop.Dock = DockStyle.Top;
      this.panelTop.ForeColor = Color.Gray;
      this.panelTop.Location = new Point(0, 0);
      this.panelTop.Name = "panelTop";
      this.panelTop.Size = new Size(900, 71);
      this.panelTop.TabIndex = 19;
      this.pnlTabBtns.Controls.Add((Control) this.panel1);
      this.pnlTabBtns.Controls.Add((Control) this.btnProperties);
      this.pnlTabBtns.Controls.Add((Control) this.btnNavigationPropertyOneToManyTab);
      this.pnlTabBtns.Controls.Add((Control) this.btnEntityInformation);
      this.pnlTabBtns.Controls.Add((Control) this.panel11);
      this.pnlTabBtns.Controls.Add((Control) this.btnNavigationProperties);
      this.pnlTabBtns.Controls.Add((Control) this.panel10);
      this.pnlTabBtns.Controls.Add((Control) this.panel9);
      this.pnlTabBtns.Location = new Point(150, 0);
      this.pnlTabBtns.Name = "pnlTabBtns";
      this.pnlTabBtns.Size = new Size(785, 75);
      this.pnlTabBtns.TabIndex = 31;
      this.panel1.BackColor = Color.White;
      this.panel1.Location = new Point(587, 66);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(189, 5);
      this.panel1.TabIndex = 30;
      this.panel1.Tag = (object) "3";
      this.panel1.Visible = false;
      this.btnProperties.BackColor = Color.FromArgb(83, 83, 84);
      this.btnProperties.BackgroundImageLayout = ImageLayout.None;
      this.btnProperties.Cursor = Cursors.Hand;
      this.btnProperties.FlatAppearance.BorderSize = 0;
      this.btnProperties.FlatStyle = FlatStyle.Flat;
      this.btnProperties.Font = new Font("Segoe UI", 12f);
      this.btnProperties.ForeColor = Color.Silver;
      this.btnProperties.Location = new Point(195, 0);
      this.btnProperties.Name = "btnProperties";
      this.btnProperties.Size = new Size(190, 68);
      this.btnProperties.TabIndex = 24;
      this.btnProperties.Tag = (object) "1";
      this.btnProperties.Text = "Properties";
      this.btnProperties.UseVisualStyleBackColor = false;
      this.btnProperties.Click += new EventHandler(this.btnProperties_Click);
      this.btnNavigationPropertyOneToManyTab.BackColor = Color.FromArgb(83, 83, 84);
      this.btnNavigationPropertyOneToManyTab.BackgroundImageLayout = ImageLayout.None;
      this.btnNavigationPropertyOneToManyTab.Cursor = Cursors.Hand;
      this.btnNavigationPropertyOneToManyTab.FlatAppearance.BorderSize = 0;
      this.btnNavigationPropertyOneToManyTab.FlatStyle = FlatStyle.Flat;
      this.btnNavigationPropertyOneToManyTab.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
      this.btnNavigationPropertyOneToManyTab.ForeColor = Color.Silver;
      this.btnNavigationPropertyOneToManyTab.Location = new Point(587, 0);
      this.btnNavigationPropertyOneToManyTab.Name = "btnNavigationPropertyOneToManyTab";
      this.btnNavigationPropertyOneToManyTab.Size = new Size(190, 68);
      this.btnNavigationPropertyOneToManyTab.TabIndex = 29;
      this.btnNavigationPropertyOneToManyTab.Tag = (object) "3";
      this.btnNavigationPropertyOneToManyTab.Text = "Navigation Property (1-Many) *";
      this.btnNavigationPropertyOneToManyTab.UseVisualStyleBackColor = false;
      this.btnNavigationPropertyOneToManyTab.Visible = false;
      this.btnNavigationPropertyOneToManyTab.Click += new EventHandler(this.btnChildEntitiesTab_Click);
      this.btnEntityInformation.BackColor = Color.FromArgb(83, 83, 84);
      this.btnEntityInformation.BackgroundImageLayout = ImageLayout.None;
      this.btnEntityInformation.Cursor = Cursors.Hand;
      this.btnEntityInformation.FlatAppearance.BorderSize = 0;
      this.btnEntityInformation.FlatStyle = FlatStyle.Flat;
      this.btnEntityInformation.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
      this.btnEntityInformation.ForeColor = Color.White;
      this.btnEntityInformation.Location = new Point(0, 0);
      this.btnEntityInformation.Name = "btnEntityInformation";
      this.btnEntityInformation.Size = new Size(190, 68);
      this.btnEntityInformation.TabIndex = 23;
      this.btnEntityInformation.Tag = (object) "0";
      this.btnEntityInformation.Text = "Entity Information";
      this.btnEntityInformation.UseVisualStyleBackColor = false;
      this.btnEntityInformation.Click += new EventHandler(this.btnEntityInformation_Click);
      this.panel11.BackColor = Color.White;
      this.panel11.Location = new Point(391, 66);
      this.panel11.Name = "panel11";
      this.panel11.Size = new Size(189, 5);
      this.panel11.TabIndex = 28;
      this.panel11.Tag = (object) "2";
      this.panel11.Visible = false;
      this.btnNavigationProperties.BackColor = Color.FromArgb(83, 83, 84);
      this.btnNavigationProperties.BackgroundImageLayout = ImageLayout.None;
      this.btnNavigationProperties.Cursor = Cursors.Hand;
      this.btnNavigationProperties.FlatAppearance.BorderSize = 0;
      this.btnNavigationProperties.FlatStyle = FlatStyle.Flat;
      this.btnNavigationProperties.Font = new Font("Segoe UI", 12f);
      this.btnNavigationProperties.ForeColor = Color.Silver;
      this.btnNavigationProperties.Location = new Point(391, 0);
      this.btnNavigationProperties.Name = "btnNavigationProperties";
      this.btnNavigationProperties.Size = new Size(190, 68);
      this.btnNavigationProperties.TabIndex = 25;
      this.btnNavigationProperties.Tag = (object) "2";
      this.btnNavigationProperties.Text = "Navigation Properties";
      this.btnNavigationProperties.UseVisualStyleBackColor = false;
      this.btnNavigationProperties.Click += new EventHandler(this.btnNavigationProperties_Click);
      this.panel10.BackColor = Color.White;
      this.panel10.Location = new Point(195, 66);
      this.panel10.Name = "panel10";
      this.panel10.Size = new Size(189, 5);
      this.panel10.TabIndex = 27;
      this.panel10.Tag = (object) "1";
      this.panel10.Visible = false;
      this.panel9.BackColor = Color.White;
      this.panel9.Location = new Point(1, 66);
      this.panel9.Name = "panel9";
      this.panel9.Size = new Size(189, 5);
      this.panel9.TabIndex = 26;
      this.panel9.Tag = (object) "0";
      this.pnlBottom.BackColor = Color.FromArgb(83, 83, 84);
      this.pnlBottom.Controls.Add((Control) this.btnCancel);
      this.pnlBottom.Controls.Add((Control) this.btnGenerate);
      this.pnlBottom.Dock = DockStyle.Bottom;
      this.pnlBottom.ForeColor = Color.Gray;
      this.pnlBottom.Location = new Point(0, 512);
      this.pnlBottom.Name = "pnlBottom";
      this.pnlBottom.Size = new Size(900, 79);
      this.pnlBottom.TabIndex = 20;
      this.btnCancel.BackColor = Color.Gray;
      this.btnCancel.Cursor = Cursors.Hand;
      this.btnCancel.DialogResult = DialogResult.Cancel;
      this.btnCancel.FlatAppearance.BorderColor = Color.Gray;
      this.btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(3, 184, 120);
      this.btnCancel.FlatStyle = FlatStyle.Flat;
      this.btnCancel.Font = new Font("Microsoft Sans Serif", 7f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.btnCancel.ForeColor = Color.White;
      this.btnCancel.Location = new Point(760, 39);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new Size(136, 23);
      this.btnCancel.TabIndex = 19;
      this.btnCancel.Text = "invisible cancel button";
      this.btnCancel.UseVisualStyleBackColor = false;
      this.btnCancel.Visible = false;
      this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
      this.panel3.Controls.Add((Control) this.tabContainer);
      this.panel3.Dock = DockStyle.Fill;
      this.panel3.Location = new Point(0, 71);
      this.panel3.Name = "panel3";
      this.panel3.Size = new Size(900, 441);
      this.panel3.TabIndex = 21;
      this.AcceptButton = (IButtonControl) this.btnGenerate;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = Color.FromArgb(38, 38, 42);
      this.CancelButton = (IButtonControl) this.btnCancel;
      this.ClientSize = new Size(900, 591);
      this.Controls.Add((Control) this.panel3);
      this.Controls.Add((Control) this.pnlBottom);
      this.Controls.Add((Control) this.panelTop);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (EntityGeneratorForm);
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "ASP.NET Zero - Entity Generator";
      this.Load += new EventHandler(this.EntityGeneratorForm_Load);
      this.tabContainer.ResumeLayout(false);
      this.EntityInfoTab.ResumeLayout(false);
      this.EntityInfoTab.PerformLayout();
      this.pnlUserInterface.ResumeLayout(false);
      this.pnlUserInterface.PerformLayout();
      this.pnlMultiTenancy.ResumeLayout(false);
      this.pnlMultiTenancy.PerformLayout();
      this.pnlAuditing.ResumeLayout(false);
      this.pnlAuditing.PerformLayout();
      this.pnlDatabaseMigrations.ResumeLayout(false);
      this.pnlDatabaseMigrations.PerformLayout();
      this.pnlEntityInfo.ResumeLayout(false);
      this.pnlEntityInfo.PerformLayout();
      this.tabPage2.ResumeLayout(false);
      this.panel12.ResumeLayout(false);
      this.panel12.PerformLayout();
      this.NavigationTab.ResumeLayout(false);
      this.panel13.ResumeLayout(false);
      this.panel13.PerformLayout();
      this.NavigationPropertyOneToManyTab.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.panelTop.ResumeLayout(false);
      this.pnlTabBtns.ResumeLayout(false);
      this.pnlBottom.ResumeLayout(false);
      this.panel3.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
