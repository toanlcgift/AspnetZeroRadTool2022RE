using AspNetZeroRadToolVisualStudioExtension.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace AspNetZeroRadToolVisualStudioExtension
{
  public static class RadToolFileUpdater
  {
    public static bool CopyRadToolDlls(string soltionFileDirectoryPath)
    {
      try
      {
        string str = soltionFileDirectoryPath + "\\RadTool.zip";
        RadToolFileUpdater.CopyStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("AspNetZeroRadToolVisualStudioExtension.Resources.AspNetZeroRadTool.zip"), (Stream) File.Create(str));
        RadToolFileUpdater.DeleteExistingFiles(soltionFileDirectoryPath);
        ZipFile.ExtractToDirectory(str, soltionFileDirectoryPath);
        File.Delete(str);
        return true;
      }
      catch (Exception ex)
      {
        MsgBox.Exception("Error while copying Power Tools DLLs", ex);
        return false;
      }
    }

    private static void CopyStream(Stream input, Stream output)
    {
      byte[] buffer = new byte[8192];
      int count;
      while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
        output.Write(buffer, 0, count);
      output.Close();
    }

    private static void DeleteExistingFiles(string soltionFileDirectoryPath)
    {
      string path1 = soltionFileDirectoryPath + "\\AspNetZeroRadTool";
      foreach (string path2 in ((IEnumerable<string>) Directory.GetFiles(path1)).Where<string>((Func<string, bool>) (f => !f.EndsWith(".json") && !f.EndsWith(".config") || Path.GetFileName(f).Contains("ZeroRadTool"))))
        File.Delete(path2);
      try
      {
        foreach (string path3 in ((IEnumerable<string>) Directory.GetDirectories(path1)).Where<string>((Func<string, bool>) (d => !d.Contains("FileTemplates"))))
          Directory.Delete(path3, true);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
