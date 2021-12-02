using Newtonsoft.Json;
using System.Collections.Generic;

namespace AspNetZeroRadToolVisualStudioExtension.EntityInfo
{
  public class NavigationProperty
  {
    public string Namespace { get; set; }

    [JsonIgnore]
    public string FilePath { get; set; }

    public string ForeignEntityName { get; set; }

    public string IdType { get; set; }

    public bool IsNullable { get; set; }

    public string PropertyName { get; set; }

    public string DisplayPropertyName { get; set; }

    public int DuplicationNumber { get; set; }

    public string RelationType { get; set; }

    public string ViewType { get; set; }

    [JsonIgnore]
    public List<string> DisplayNameList { get; set; }
  }
}
