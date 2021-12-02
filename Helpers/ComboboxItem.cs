namespace AspNetZeroRadToolVisualStudioExtension.Helpers
{
  public class ComboboxItem
  {
    public string Text { get; set; }

    public object Value { get; set; }

    public override string ToString() => this.Text;
  }
}
