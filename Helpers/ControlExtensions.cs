using System.Drawing;
using System.Windows.Forms;

namespace AspNetZeroRadToolVisualStudioExtension.Helpers
{
  public static class ControlExtensions
  {
    public static void SetFontStyleAsBold(this Control ctrl) => ctrl.Font = new Font(ctrl.Font, FontStyle.Bold);

    public static void SetFontStyleAsRegular(this Control ctrl) => ctrl.Font = new Font(ctrl.Font, FontStyle.Regular);
  }
}
