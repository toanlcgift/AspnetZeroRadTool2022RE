using System;

namespace AspNetZeroRadToolVisualStudioExtension.Helpers
{
  public class ProjectVersionHelper
  {
    public static int ProjectVersionToNumber(string version)
    {
      if (string.IsNullOrEmpty(version))
        return 0;
      if (version.Contains("rc"))
        version = version.Substring(0, version.IndexOf('-'));
      string[] strArray = version.Replace("v", "").Split('.');
      string str1 = "";
      if (strArray.Length == 1)
        strArray = new string[3]{ strArray[0], "0", "0" };
      else if (strArray.Length == 2)
        strArray = new string[3]
        {
          strArray[0],
          strArray[1],
          "0"
        };
      foreach (string str2 in strArray)
      {
        string str3 = "";
        for (int index = 0; index < 2 - str2.Length; ++index)
          str3 += "0";
        str1 = str1 + str3 + str2;
      }
      try
      {
        return Convert.ToInt32(str1);
      }
      catch (Exception ex)
      {
        return 0;
      }
    }
  }
}
