using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using AspNetZeroRadToolVisualStudioExtension.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs.EntityLoaders.Database
{
  public class EntityLoaderFromMySqlDatabase : EntityLoaderFromDatabase
  {
    private List<Entity> GetTableAsEntity(DataTable table, Entity baseEntity)
    {
      List<Entity> entityList = new List<Entity>();
      Entity entity = this.ApplyBaseEntity("", baseEntity);
      string str = "";
      bool flag = false;
      foreach (DataRow row in (InternalDataCollectionBase) table.Rows)
      {
        int num = 0;
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
        foreach (DataColumn column in (InternalDataCollectionBase) table.Columns)
        {
          if (column.ColumnName.ToLower().Equals("table_name") && !row[column].ToString().ToLower().Equals(str))
          {
            if (!string.IsNullOrWhiteSpace(str))
              entityList.Add(entity);
            str = row[column].ToString();
            entity = this.ApplyBaseEntity(row[column].ToString(), baseEntity);
          }
          if (column.ColumnName.ToLower().Equals("column_name"))
          {
            if (row[column].ToString().Equals("Id"))
            {
              flag = true;
              continue;
            }
            property.Name = row[column].ToString();
          }
          if (column.ColumnName.ToLower().Equals("data_type"))
          {
            if (flag)
            {
              entity.PrimaryKeyType = this.ConvertToCsharpType(row[column].ToString());
              continue;
            }
            property.Type = this.ConvertToCsharpType(row[column].ToString());
          }
          if (column.ColumnName.ToLower().Equals("is_nullable"))
            property.Nullable = !row[column].ToString().ToLower().Equals("no");
          int result;
          if (column.ColumnName.ToLower().Equals("character_maximum_length") && int.TryParse(row[column].ToString(), out result) && result != int.MaxValue)
            num = result;
        }
        if (flag)
        {
          flag = false;
        }
        else
        {
          if (property.Type == "string" && num != 0)
          {
            property.MinLength = 0;
            property.MaxLength = num;
          }
          entity.Properties.Add(property);
        }
      }
      entityList.Add(entity);
      return entityList;
    }

    public override List<Entity> GetTablesAsEntity(
      string connectionString,
      Entity baseEntity)
    {
      try
      {
        using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
        {
          mySqlConnection.Open();
          return this.GetTableAsEntity(mySqlConnection.GetSchema("columns"), baseEntity);
        }
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Cannot get table list from MySQL database!", ex);
        return (List<Entity>) null;
      }
    }

    public override bool TestConnection(string connectionString, out Exception exception)
    {
      using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
      {
        try
        {
          mySqlConnection.Open();
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
  }
}
