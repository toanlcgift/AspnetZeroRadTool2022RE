using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs.Providers
{
  public static class EnumProvider
  {
    private static string _slnSrcDirectory = "";

    public static List<EnumDefinition> EnumList { get; set; }

    public static void SetSolutionPathForEnums(string solutionPath) => EnumProvider._slnSrcDirectory = solutionPath;

    public static void StoreAllEnumDefinitionsInSolution()
    {
      List<string> filesUnderDirectory = EnumProvider.GetAllCsFilesUnderDirectory(EnumProvider._slnSrcDirectory, new List<string>());
      List<EnumDefinition> enumList = new List<EnumDefinition>();
      foreach (string csFile in filesUnderDirectory)
      {
        try
        {
          enumList = EnumProvider.GetEnumsFromFile(csFile, enumList);
        }
        catch (Exception ex)
        {
        }
      }
      EnumProvider.EnumList = enumList;
    }

    private static List<EnumDefinition> GetEnumsFromFile(
      string csFile,
      List<EnumDefinition> enumList)
    {
      foreach (NamespaceDeclarationSyntax declarationSyntax1 in CSharpSyntaxTree.ParseText(File.ReadAllText(csFile)).GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>())
      {
        IEnumerable<EnumDeclarationSyntax> source = declarationSyntax1.DescendantNodes().OfType<EnumDeclarationSyntax>();
        if (!(source is EnumDeclarationSyntax[] declarationSyntaxArray3))
          declarationSyntaxArray3 = source.ToArray<EnumDeclarationSyntax>();
        EnumDeclarationSyntax[] declarationSyntaxArray2 = declarationSyntaxArray3;
        if (((IEnumerable<EnumDeclarationSyntax>) declarationSyntaxArray2).Any<EnumDeclarationSyntax>())
        {
          foreach (EnumDeclarationSyntax declarationSyntax2 in declarationSyntaxArray2)
          {
            EnumDefinition enumDefinition = new EnumDefinition()
            {
              Name = declarationSyntax2.Identifier.ToString().Trim(),
              Namespace = declarationSyntax1.Name.ToString().Trim(),
              EnumProperties = new List<EnumProperty>()
            };
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = declarationSyntax2.Members;
            int num = 0;
            foreach (EnumMemberDeclarationSyntax declarationSyntax3 in members)
            {
              enumDefinition.EnumProperties.Add(new EnumProperty()
              {
                Name = declarationSyntax3.Identifier.ToString().Trim(),
                Value = Convert.ToInt32(declarationSyntax3.EqualsValue?.Value.ToString().Trim() ?? num.ToString())
              });
              ++num;
            }
            enumList.Add(enumDefinition);
          }
        }
      }
      return enumList;
    }

    private static List<string> GetAllCsFilesUnderDirectory(
      string path,
      List<string> allCsFileList)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(path);
      foreach (string str in ((IEnumerable<FileInfo>) directoryInfo.GetFiles("*.cs")).Select<FileInfo, string>((Func<FileInfo, string>) (f => f.DirectoryName + "\\" + f.Name)).ToList<string>())
        allCsFileList.Add(str);
      foreach (string path1 in ((IEnumerable<DirectoryInfo>) directoryInfo.GetDirectories()).Select<DirectoryInfo, string>((Func<DirectoryInfo, string>) (d => path + "\\" + d.Name)).ToList<string>())
        allCsFileList = EnumProvider.GetAllCsFilesUnderDirectory(path1, allCsFileList);
      return allCsFileList;
    }
  }
}
