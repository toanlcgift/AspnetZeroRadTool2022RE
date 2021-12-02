using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs.Providers
{
  public class CsFileProvider
  {
    public static List<string> GetAllCsFilesUnderDirectory(
      string path,
      List<string> allCsFileList)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(path);
      foreach (string str in ((IEnumerable<FileInfo>) directoryInfo.GetFiles("*.cs")).Select<FileInfo, string>((Func<FileInfo, string>) (f => f.DirectoryName + "\\" + f.Name)).ToList<string>())
        allCsFileList.Add(str);
      foreach (string path1 in ((IEnumerable<DirectoryInfo>) directoryInfo.GetDirectories()).Select<DirectoryInfo, string>((Func<DirectoryInfo, string>) (d => path + "\\" + d.Name)).ToList<string>())
        allCsFileList = CsFileProvider.GetAllCsFilesUnderDirectory(path1, allCsFileList);
      return allCsFileList;
    }
  }
}
