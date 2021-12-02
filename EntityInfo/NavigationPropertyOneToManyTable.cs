using System;

namespace AspNetZeroRadToolVisualStudioExtension.EntityInfo
{
  [Serializable]
  public class NavigationPropertyOneToManyTable
  {
    public string EntityJson { get; set; }

    public string ForeignPropertyName { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsNullable { get; set; }

    public string DisplayPropertyName { get; set; }

    public string ViewType { get; set; }
  }
}
