using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using AspNetZeroRadToolVisualStudioExtension.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs.EntityLoaders.Database
{
  public class EntityLoaderFromPostgreSqlDatabase : EntityLoaderFromDatabase
  {
    public override bool TestConnection(string connectionString, out Exception exception)
    {
      using (NpgsqlConnection npgsqlConnection = new NpgsqlConnection(connectionString))
      {
        try
        {
          npgsqlConnection.Open();
          exception = (Exception) null;
          return true;
        }
        catch (Exception ex)
        {
          exception = ex;
          return false;
        }
      }
    }

    public override List<Entity> GetTablesAsEntity(
      string connectionString,
      Entity baseEntity)
    {
      List<Entity> entityList = new List<Entity>();
      try
      {
        entityList = this.CreateEntityInformationListFromTableInformationList(EntityLoaderFromPostgreSqlDatabase.GetTableInformationList(connectionString), baseEntity);
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Cannot convert tables to entities!", ex);
      }
      return entityList;
    }

    private static List<PostgreColumnInformation> GetTableInformationList(
      string connectionString)
    {
      List<PostgreColumnInformation> columnInformationList = new List<PostgreColumnInformation>();
      using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
      {
        connection.Open();
        using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand("\n                        SELECT \n                        *\n                        FROM \n                           information_schema.columns\n                        WHERE \n                          (table_schema != 'information_schema' AND table_schema != 'pg_catalog')\n                    ", connection))
        {
          NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader(CommandBehavior.Default);
          while (npgsqlDataReader.Read())
          {
            PostgreColumnInformation columnInformation = new PostgreColumnInformation();
            columnInformation.TableName = npgsqlDataReader["table_name"].ToString();
            columnInformation.ColumnName = npgsqlDataReader["column_name"].ToString();
            columnInformation.IsNullable = npgsqlDataReader["is_nullable"].ToString() == "YES";
            columnInformation.DataType = npgsqlDataReader["data_type"].ToString();
            string s = npgsqlDataReader["character_maximum_length"].ToString();
            columnInformation.CharacterMaxLength = string.IsNullOrEmpty(s) ? 0 : int.Parse(s);
            columnInformationList.Add(columnInformation);
          }
          connection.Close();
        }
      }
      return columnInformationList;
    }

    private List<Entity> CreateEntityInformationListFromTableInformationList(
      List<PostgreColumnInformation> tableInformationList,
      Entity baseEntity)
    {
      List<Entity> entityList = new List<Entity>();
      foreach (IGrouping<string, PostgreColumnInformation> source in tableInformationList.GroupBy<PostgreColumnInformation, string>((Func<PostgreColumnInformation, string>) (info => info.TableName)))
      {
        Entity entity = this.ApplyBaseEntity(source.Key, baseEntity);
        foreach (PostgreColumnInformation columnInformation in source.ToList<PostgreColumnInformation>())
        {
          if (columnInformation.ColumnName == "Id")
          {
            entity.PrimaryKeyType = this.PostgreDataTypeToCsharpType(columnInformation.DataType);
          }
          else
          {
            Property property = new Property()
            {
              Name = "",
              Type = "",
              Regex = "",
              Range = new NumericalRange(),
              UserInterface = new PropertyUserInterfaceInfo()
              {
                AdvancedFilter = true,
                CreateOrUpdate = true,
                List = true
              }
            };
            property.Name = columnInformation.ColumnName;
            property.Nullable = columnInformation.IsNullable;
            property.MaxLength = columnInformation.CharacterMaxLength;
            property.Type = this.PostgreDataTypeToCsharpType(columnInformation.DataType);
            entity.Properties.Add(property);
          }
        }
        entityList.Add(entity);
      }
      return entityList;
    }

    private string PostgreDataTypeToCsharpType(string dataType)
    {
      switch (dataType.ToLower())
      {
        case "bigint":
          return "long";
        case "boolean":
          return "bool";
        case "character":
        case "character varying":
        case "json":
        case "text":
          return "string";
        case "date":
        case "time with time zone":
        case "time without time zone":
          return "DateTime";
        case "decimal":
        case "numeric":
          return "decimal";
        case "integer":
          return "int";
        case "real":
          return "double";
        case "smallint":
          return "short";
        case "uuid":
          return "Guid";
        default:
          return "string";
      }
    }
  }
}
