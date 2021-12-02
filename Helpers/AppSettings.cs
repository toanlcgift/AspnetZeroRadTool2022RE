using AspNetZeroRadToolVisualStudioExtension;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AspNetZeroRadToolVisualStudioExtension.Helpers
{
  public class AppSettings
  {
    public static string AppName = "ASP.NET Zero Power Tools";
    public static string PowerToolsDirectoryName = "AspNetZeroRadTool";
    public static string[] PropertyTypes = new string[11]
    {
      "bool",
      "byte",
      "short",
      "DateTime",
      "decimal",
      "double",
      "Guid",
      "int",
      "long",
      "string",
      "enum"
    };

    public static string GetExtensionInstallationDirectoryOrNull()
    {
      try
      {
        return Path.GetDirectoryName(new Uri(typeof (AspNetZeroRadToolPackage).Assembly.CodeBase, UriKind.Absolute).LocalPath);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.Message, ex);
        return (string) null;
      }
    }

    public static string GetAppVersion()
    {
      try
      {
        string installationDirectoryOrNull = AppSettings.GetExtensionInstallationDirectoryOrNull();
        if (installationDirectoryOrNull == null)
          return "0.0.0";
        string str = Path.Combine(installationDirectoryOrNull, "extension.vsixmanifest");
        if (!File.Exists(str))
          return "0.0.0";
        XDocument xdocument = XDocument.Load(str);
        if (xdocument.Root == null)
          return "0.0.0";
        XElement xelement = (XElement) xdocument.Root.Nodes().FirstOrDefault<XNode>((Func<XNode, bool>) (node => ((XElement) node).Name.LocalName == "Metadata"));
        XAttribute xattribute = (xelement != null ? xelement.Elements().FirstOrDefault<XElement>((Func<XElement, bool>) (node => node.Name.LocalName == "Identity")) : (XElement) null)?.Attribute((XName) "Version");
        return xattribute != null ? xattribute.Value : "0.0.0";
      }
      catch (Exception ex)
      {
        Logger.Error(ex.Message, ex);
        return "0.0.0";
      }
    }
  }
}
