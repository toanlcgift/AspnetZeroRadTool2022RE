using Newtonsoft.Json;
using System.Collections.Generic;

namespace AspNetZeroRadToolVisualStudioExtension.EntityInfo
{
  public class Entity
  {
    [JsonIgnore]
    public string RootPath { get; set; }

    public bool IsRegenerate { get; set; }

    [JsonIgnore]
    public bool IsLoadedFromDatabase { get; set; }

    [JsonIgnore]
    public string Namespace => !string.IsNullOrEmpty(this.CompanyName) ? this.CompanyName + "." + this.ProjectName : this.ProjectName;

    [JsonIgnore]
    public string CompanyName { get; set; }

    [JsonIgnore]
    public string ProjectType { get; set; }

    [JsonIgnore]
    public string AppAreaName { get; set; }

    [JsonIgnore]
    public string ProjectVersion { get; set; }

    [JsonIgnore]
    public bool EntityHistoryDisabled { get; set; }

    public string MenuPosition { get; set; }

    [JsonIgnore]
    public string ProjectName { get; set; }

    public string RelativeNamespace { get; set; }

    public string EntityName { get; set; }

    public string EntityNamePlural { get; set; }

    public string TableName { get; set; }

    public string PrimaryKeyType { get; set; }

    public string BaseClass { get; set; }

    public bool EntityHistory { get; set; }

    public bool AutoMigration { get; set; }

    public bool UpdateDatabase { get; set; }

    public bool CreateUserInterface { get; set; }

    public bool CreateViewOnly { get; set; }

    public bool CreateExcelExport { get; set; }

    public bool IsNonModalCRUDPage { get; set; }

    public bool IsMasterDetailPage { get; set; }

    public PagePermissionInfo PagePermission { get; set; }

    public List<Property> Properties { get; set; }

    public List<NavigationProperty> NavigationProperties { get; set; }

    public List<NavigationPropertyOneToManyTable> NavigationPropertyOneToManyTables { get; set; } = new List<NavigationPropertyOneToManyTable>();

    public List<EnumDefinition> EnumDefinitions { get; set; }

    public DbContextDefinition DbContext { get; set; }
  }
}
