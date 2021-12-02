// Decompiled with JetBrains decompiler
// Type: AspNetZeroRadToolVisualStudioExtension.VSPackage
// Assembly: AspNetZeroRadToolVisualStudioExtension, Version=1.0.0.0, Culture=neutral, PublicKeyToken=ed5f8fd45678350e
// MVID: 5457A8AF-DCE5-4FDD-9F2A-43CCB5D6BA98
// Assembly location: C:\Users\ToanND\Downloads\AspNetZeroRadToolVisualStudioExtension\AspNetZeroRadToolVisualStudioExtension.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetZeroRadToolVisualStudioExtension
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class VSPackage
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal VSPackage()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (VSPackage.resourceMan == null)
          VSPackage.resourceMan = new ResourceManager("AspNetZeroRadToolVisualStudioExtension.VSPackage", typeof (VSPackage).Assembly);
        return VSPackage.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => VSPackage.resourceCulture;
      set => VSPackage.resourceCulture = value;
    }

    internal static string _110 => VSPackage.ResourceManager.GetString("110", VSPackage.resourceCulture);

    internal static string _112 => VSPackage.ResourceManager.GetString("112", VSPackage.resourceCulture);

    internal static Icon _400 => (Icon) VSPackage.ResourceManager.GetObject("400", VSPackage.resourceCulture);

    internal static Bitmap AspNetZeroLogo => (Bitmap) VSPackage.ResourceManager.GetObject(nameof (AspNetZeroLogo), VSPackage.resourceCulture);

    internal static Bitmap editicon18tr => (Bitmap) VSPackage.ResourceManager.GetObject(nameof (editicon18tr), VSPackage.resourceCulture);
  }
}
