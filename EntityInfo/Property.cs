namespace AspNetZeroRadToolVisualStudioExtension.EntityInfo
{
  public class Property
  {
    public string Name { get; set; }

    public string Type { get; set; }

    public int MaxLength { get; set; }

    public int MinLength { get; set; }

    public NumericalRange Range { get; set; }

    public bool Required { get; set; }

    public bool Nullable { get; set; }

    public string Regex { get; set; }

    public PropertyUserInterfaceInfo UserInterface { get; set; }

    public bool IsNumerical() => this.Type == "int" || this.Type == "long" || this.Type == "byte" || this.Type == "short" || this.Type == "double" || this.Type == "decimal";

    public bool IsDecimalNumber() => this.Type == "double" || this.Type == "decimal";
  }
}
