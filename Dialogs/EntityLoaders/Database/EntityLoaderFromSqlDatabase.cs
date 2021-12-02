using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using AspNetZeroRadToolVisualStudioExtension.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs.EntityLoaders.Database
{
  public class EntityLoaderFromSqlDatabase : EntityLoaderFromDatabase
  {
    public override List<Entity> GetTablesAsEntity(
      string connectionString,
      Entity baseEntity)
    {
      List<Entity> entityList = new List<Entity>();
      try
      {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          if (connection.State == ConnectionState.Closed)
            connection.Open();
          foreach (DataRow row in (InternalDataCollectionBase) connection.GetSchema("Tables").Rows)
          {
            Entity tableAsEntity = this.GetTableAsEntity((string) row[2], (string) row[1], baseEntity, connection);
            if (tableAsEntity != null)
              entityList.Add(tableAsEntity);
          }
        }
        return entityList;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Cannot convert tables to entities!", ex);
      }
      return entityList;
    }

    private Entity GetTableAsEntity(
      string targetTableName,
      string targetSchemaName,
      Entity baseEntity,
      SqlConnection connection)
    {
      try
      {
        Entity entity = this.ApplyBaseEntity(targetTableName, baseEntity);
        SqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM [" + targetSchemaName + "].[" + targetTableName + "] WHERE 1 = 0";
        command.CommandType = CommandType.Text;
        using (SqlDataReader sqlDataReader = command.ExecuteReader())
        {
          DataTable schemaTable = sqlDataReader.GetSchemaTable();
          if (schemaTable == null)
            return (Entity) null;
          foreach (DataRow row in (InternalDataCollectionBase) schemaTable.Rows)
          {
            if ((row["ColumnName"] ?? (object) "").ToString().Equals("Id"))
              entity.PrimaryKeyType = this.ConvertSqlTypeToCsharpType(row["Datatype"].ToString());
            if (!(row["ColumnName"] ?? (object) "").ToString().EndsWith("Id"))
            {
              Property property = new Property()
              {
                Name = (row["ColumnName"] ?? (object) "").ToString(),
                Type = this.ConvertSqlTypeToCsharpType((row["Datatype"] ?? (object) "").ToString()),
                MaxLength = -1,
                MinLength = -1,
                Required = !(bool) (row["AllowDBNull"] ?? (object) true),
                Nullable = (bool) (row["AllowDBNull"] ?? (object) false),
                Regex = "",
                Range = new NumericalRange(),
                UserInterface = new PropertyUserInterfaceInfo()
                {
                  List = true,
                  CreateOrUpdate = true,
                  AdvancedFilter = true
                }
              };
              int result;
              if (property.Type == "string" && row["ColumnSize"] != null && int.TryParse(row["ColumnSize"].ToString(), out result) && result != int.MaxValue)
              {
                property.MaxLength = result;
                property.MinLength = 0;
              }
              entity.Properties.Add(property);
            }
          }
        }
        return entity;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Cannot convert table to entity!", ex);
        return (Entity) null;
      }
    }

    public override bool TestConnection(string connectionString, out Exception exception)
    {
      using (SqlConnection sqlConnection = new SqlConnection(connectionString))
      {
        try
        {
          sqlConnection.Open();
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

    protected string ConvertSqlTypeToCsharpType(string sqlType)
    {
      try
      {
        sqlType = sqlType.Split('.')[1].ToLower();
      }
      catch (Exception ex)
      {
        return "string";
      }
      return this.ConvertToCsharpType(sqlType);
    }
  }
}
