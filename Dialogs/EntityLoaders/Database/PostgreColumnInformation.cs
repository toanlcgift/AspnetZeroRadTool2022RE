namespace AspNetZeroRadToolVisualStudioExtension.Dialogs.EntityLoaders.Database
{
  internal class PostgreColumnInformation
  {
    public string TableName { get; set; }

    public string ColumnName { get; set; }

    public bool IsNullable { get; set; }

    public string DataType { get; set; }

    public int CharacterMaxLength { get; set; }
  }
}
