using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs.EntityLoaders.Database
{
  public abstract class EntityLoaderFromDatabase
  {
    protected string GetConnectionString(string slnFolder) => (string) ((JObject) JObject.Parse(File.ReadAllText((((IEnumerable<string>) Directory.GetDirectories(slnFolder + "\\src")).Any<string>((Func<string, bool>) (d => d.ToLower().EndsWith(".mvc"))) ? ((IEnumerable<string>) Directory.GetDirectories(slnFolder + "\\src")).First<string>((Func<string, bool>) (d => d.ToLower().EndsWith(".mvc"))) : ((IEnumerable<string>) Directory.GetDirectories(slnFolder + "\\src")).First<string>((Func<string, bool>) (d => d.ToLower().EndsWith(".host")))) + "\\appsettings.json"))["ConnectionStrings"])["Default"];

    protected string ConvertToCsharpType(string sqlType)
    {
      switch (sqlType.ToLower())
      {
        case "bigint":
        case "int64":
          return "long";
        case "bit":
        case "bool":
        case "boolean":
          return "bool";
        case "byte":
        case "int8":
        case "tinyint":
          return "byte";
        case "date":
        case "datetime":
        case "datetime2":
        case "datetimeoffset":
        case "smalldatetime":
        case "time":
        case "timestamp":
        case "year":
          return "DateTime";
        case "decimal":
          return "decimal";
        case "double":
        case "float":
        case "real":
          return "double";
        case "enum":
        case "int":
        case "int32":
        case "mediumint":
          return "int";
        case "guid":
        case "uniqueidentifier":
          return "Guid";
        case "int16":
        case "smallint":
          return "short";
        default:
          return "string";
      }
    }

    public Entity ApplyBaseEntity(string targetTableName, Entity baseEntity) => new Entity()
    {
      IsLoadedFromDatabase = true,
      RootPath = baseEntity.RootPath,
      CompanyName = baseEntity.CompanyName,
      ProjectName = baseEntity.ProjectName,
      AppAreaName = baseEntity.AppAreaName,
      EnumDefinitions = new List<EnumDefinition>(),
      NavigationProperties = new List<NavigationProperty>(),
      MenuPosition = "main",
      PrimaryKeyType = "int",
      BaseClass = "Entity",
      AutoMigration = true,
      UpdateDatabase = true,
      CreateUserInterface = true,
      CreateViewOnly = true,
      CreateExcelExport = true,
      PagePermission = new PagePermissionInfo()
      {
        Host = true,
        Tenant = true
      },
      EntityName = targetTableName,
      EntityNamePlural = targetTableName,
      TableName = targetTableName,
      RelativeNamespace = targetTableName,
      IsRegenerate = false,
      Properties = new List<Property>(),
      IsNonModalCRUDPage = false,
      IsMasterDetailPage = false
    };

    public abstract bool TestConnection(string connectionString, out Exception exception);

    public abstract List<Entity> GetTablesAsEntity(
      string connectionString,
      Entity baseEntity);
  }
}
