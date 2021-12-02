// Decompiled with JetBrains decompiler
// Type: AspNetZeroRadToolVisualStudioExtension.Dialogs.EntityLoaders.EntityLoaderFromJson
// Assembly: AspNetZeroRadToolVisualStudioExtension, Version=1.0.0.0, Culture=neutral, PublicKeyToken=ed5f8fd45678350e
// MVID: 5457A8AF-DCE5-4FDD-9F2A-43CCB5D6BA98
// Assembly location: C:\Users\ToanND\Downloads\AspNetZeroRadToolVisualStudioExtension\AspNetZeroRadToolVisualStudioExtension.dll

using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs.EntityLoaders
{
  public static class EntityLoaderFromJson
  {
    public static Entity Build(string entityJsonFileContent, Entity entity)
    {
      entity.IsRegenerate = true;
      JObject jobject = JObject.Parse(entityJsonFileContent);
      entity.MenuPosition = (string) jobject["MenuPosition"] ?? "";
      entity.EntityName = (string) jobject["EntityName"];
      entity.EntityNamePlural = (string) jobject["EntityNamePlural"];
      entity.RelativeNamespace = (string) jobject["RelativeNamespace"];
      entity.PrimaryKeyType = (string) jobject["PrimaryKeyType"];
      entity.BaseClass = (string) jobject["BaseClass"];
      entity.TableName = (string) jobject["TableName"];
      entity.AutoMigration = (bool) jobject["AutoMigration"];
      entity.UpdateDatabase = (bool) jobject["UpdateDatabase"];
      entity.CreateUserInterface = (bool) jobject["CreateUserInterface"];
      entity.CreateViewOnly = (bool) (jobject["CreateViewOnly"] ?? (JToken) true);
      entity.CreateExcelExport = (bool) (jobject["CreateExcelExport"] ?? (JToken) true);
      entity.EntityHistory = (bool) (jobject["EntityHistory"] ?? (JToken) true);
      entity.IsNonModalCRUDPage = (bool) (jobject["IsNonModalCRUDPage"] ?? (JToken) false);
      entity.IsMasterDetailPage = (bool) (jobject["IsMasterDetailPage"] ?? (JToken) false);
      JToken jtoken1 = jobject["PagePermission"];
      entity.PagePermission = new PagePermissionInfo()
      {
        Host = (bool) jtoken1[(object) "Host"],
        Tenant = (bool) jtoken1[(object) "Tenant"]
      };
      entity.Properties = EntityLoaderFromJson.GetProperties((JArray) jobject["Properties"]);
      if (jobject["EnumDefinitions"] != null)
        entity.EnumDefinitions = EntityLoaderFromJson.GetEnumDefinitions((JArray) jobject["EnumDefinitions"]);
      if (jobject["NavigationProperties"] != null)
        entity.NavigationProperties = EntityLoaderFromJson.GetNavigationProperties((JArray) jobject["NavigationProperties"]);
      if (jobject["NavigationPropertyOneToManyTables"] != null)
        entity.NavigationPropertyOneToManyTables = EntityLoaderFromJson.GetNavigationPropertyOneToManyTables((JArray) jobject["NavigationPropertyOneToManyTables"]);
      JToken jtoken2 = jobject["DbContext"];
      if (jtoken2 != null && jtoken2.Type != JTokenType.Null)
        entity.DbContext = new DbContextDefinition()
        {
          Name = jtoken2[(object) "Name"].ToString(),
          Namespace = jtoken2[(object) "Namespace"].ToString(),
          FullPath = jtoken2[(object) "FullPath"].ToString()
        };
      return entity;
    }

    private static List<Property> GetProperties(JArray jProperties)
    {
      List<Property> propertyList = new List<Property>();
      foreach (JToken jProperty in jProperties)
      {
        JToken jtoken1 = jProperty[(object) "UserInterface"];
        NumericalRange numericalRange = new NumericalRange();
        string type = (string) jProperty[(object) "Type"];
        if (EntityLoaderFromJson.IsNumerical(type))
        {
          JToken jtoken2 = jProperty[(object) "Range"];
          numericalRange.IsRangeSet = (bool) jtoken2[(object) "IsRangeSet"];
          if (numericalRange.IsRangeSet)
          {
            if (EntityLoaderFromJson.IsDecimalNumber(type))
            {
              try
              {
                numericalRange.MaximumValue = Convert.ToDouble((string) jtoken2[(object) "MaximumValue"]);
              }
              catch (Exception ex)
              {
                numericalRange.MaximumValue = double.MaxValue;
              }
              try
              {
                numericalRange.MinimumValue = Convert.ToDouble((string) jtoken2[(object) "MinimumValue"]);
              }
              catch (Exception ex)
              {
                numericalRange.MinimumValue = double.MinValue;
              }
            }
            else
            {
              try
              {
                numericalRange.MaximumValue = (double) Convert.ToInt32((string) jtoken2[(object) "MaximumValue"]);
              }
              catch (Exception ex)
              {
                numericalRange.MaximumValue = (double) long.MaxValue;
              }
              try
              {
                numericalRange.MinimumValue = (double) Convert.ToInt32((string) jtoken2[(object) "MinimumValue"]);
              }
              catch (Exception ex)
              {
                numericalRange.MinimumValue = (double) long.MinValue;
              }
            }
          }
        }
        propertyList.Add(new Property()
        {
          Name = (string) jProperty[(object) "Name"],
          Type = (string) jProperty[(object) "Type"],
          MaxLength = ((string) jProperty[(object) "Type"]).Equals("string") ? (int) jProperty[(object) "MaxLength"] : -1,
          MinLength = ((string) jProperty[(object) "Type"]).Equals("string") ? (int) jProperty[(object) "MinLength"] : -1,
          Required = (bool) jProperty[(object) "Required"],
          Nullable = (EntityLoaderFromJson.IsNumerical((string) jProperty[(object) "Type"]) || "DateTime".Equals((string) jProperty[(object) "Type"])) && (bool) jProperty[(object) "Nullable"],
          Regex = ((string) jProperty[(object) "Type"]).Equals("string") ? (string) jProperty[(object) "Regex"] : "",
          Range = numericalRange,
          UserInterface = new PropertyUserInterfaceInfo()
          {
            List = (bool) jtoken1[(object) "List"],
            CreateOrUpdate = (bool) jtoken1[(object) "CreateOrUpdate"],
            AdvancedFilter = (bool) (jtoken1[(object) "AdvancedFilter"] ?? (JToken) true)
          }
        });
      }
      return propertyList;
    }

    private static List<EnumDefinition> GetEnumDefinitions(JArray jEnums)
    {
      List<EnumDefinition> enumDefinitionList = new List<EnumDefinition>();
      foreach (JToken jEnum in jEnums)
      {
        EnumDefinition enumDefinition = new EnumDefinition()
        {
          Name = (string) jEnum[(object) "Name"],
          Namespace = (string) jEnum[(object) "Namespace"],
          EnumProperties = new List<EnumProperty>()
        };
        foreach (JToken jtoken in (JArray) jEnum[(object) "EnumProperties"])
          enumDefinition.EnumProperties.Add(new EnumProperty()
          {
            Name = (string) jtoken[(object) "Name"],
            Value = (int) jtoken[(object) "Value"]
          });
        enumDefinitionList.Add(enumDefinition);
      }
      return enumDefinitionList;
    }

    private static List<NavigationProperty> GetNavigationProperties(
      JArray jNavProps)
    {
      List<NavigationProperty> navigationPropertyList = new List<NavigationProperty>();
      foreach (JToken jNavProp in jNavProps)
      {
        NavigationProperty navigationProperty = new NavigationProperty()
        {
          Namespace = (string) jNavProp[(object) "Namespace"],
          ForeignEntityName = (string) jNavProp[(object) "ForeignEntityName"],
          IdType = (string) jNavProp[(object) "IdType"],
          PropertyName = (string) jNavProp[(object) "PropertyName"],
          DisplayPropertyName = (string) jNavProp[(object) "DisplayPropertyName"],
          RelationType = (string) jNavProp[(object) "RelationType"],
          IsNullable = (bool) jNavProp[(object) "IsNullable"],
          DuplicationNumber = (int) (jNavProp[(object) "DuplicationNumber"] ?? (JToken) 0),
          ViewType = (string) (jNavProp[(object) "ViewType"] ?? (JToken) "")
        };
        navigationPropertyList.Add(navigationProperty);
      }
      return navigationPropertyList;
    }

    private static List<NavigationPropertyOneToManyTable> GetNavigationPropertyOneToManyTables(
      JArray jNavProps)
    {
      List<NavigationPropertyOneToManyTable> propertyOneToManyTableList = new List<NavigationPropertyOneToManyTable>();
      foreach (JToken jNavProp in jNavProps)
      {
        NavigationPropertyOneToManyTable propertyOneToManyTable = new NavigationPropertyOneToManyTable()
        {
          EntityJson = (string) jNavProp[(object) "EntityJson"],
          ForeignPropertyName = (string) jNavProp[(object) "ForeignPropertyName"],
          ViewType = (string) jNavProp[(object) "ViewType"],
          DisplayPropertyName = (string) jNavProp[(object) "DisplayPropertyName"],
          IsDeleted = bool.Parse((string) jNavProp[(object) "IsDeleted"] ?? "false"),
          IsNullable = bool.Parse((string) jNavProp[(object) "IsNullable"] ?? "false")
        };
        propertyOneToManyTableList.Add(propertyOneToManyTable);
      }
      return propertyOneToManyTableList;
    }

    private static bool IsNumerical(string type) => type == "int" || type == "long" || type == "byte" || type == "short" || type == "double" || type == "decimal";

    private static bool IsDecimalNumber(string type) => type == "double" || type == "decimal";
  }
}
