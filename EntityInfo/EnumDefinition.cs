using System.Collections.Generic;

namespace AspNetZeroRadToolVisualStudioExtension.EntityInfo
{
  public class EnumDefinition
  {
    public string Name { get; set; }

    public string Namespace { get; set; }

    public List<EnumProperty> EnumProperties { get; set; }
  }
}
