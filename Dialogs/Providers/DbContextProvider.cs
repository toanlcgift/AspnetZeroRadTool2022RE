using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs.Providers
{
  public class DbContextProvider
  {
    public static string _slnSrcEfCoreDirectory;

    public static void SetSolutionPathForDbContexts(string solutionPath) => DbContextProvider._slnSrcEfCoreDirectory = solutionPath;

    public static List<DbContextDefinition> GetAllDbContexts()
    {
      List<string> filesUnderDirectory = CsFileProvider.GetAllCsFilesUnderDirectory(DbContextProvider._slnSrcEfCoreDirectory, new List<string>());
      List<DbContextDefinition> contextDefinitionList = new List<DbContextDefinition>();
      foreach (string csFile in filesUnderDirectory)
      {
        try
        {
          contextDefinitionList.AddRange((IEnumerable<DbContextDefinition>) DbContextProvider.GetDbContextsFromFile(csFile));
        }
        catch (Exception ex)
        {
        }
      }
      return contextDefinitionList;
    }

    private static List<DbContextDefinition> GetDbContextsFromFile(
      string csFile)
    {
      List<DbContextDefinition> contextDefinitionList = new List<DbContextDefinition>();
      foreach (NamespaceDeclarationSyntax declarationSyntax1 in CSharpSyntaxTree.ParseText(File.ReadAllText(csFile)).GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>())
      {
        IEnumerable<ClassDeclarationSyntax> source = declarationSyntax1.DescendantNodes().OfType<ClassDeclarationSyntax>();
        if (!(source is ClassDeclarationSyntax[] declarationSyntaxArray3))
          declarationSyntaxArray3 = source.ToArray<ClassDeclarationSyntax>();
        ClassDeclarationSyntax[] declarationSyntaxArray2 = declarationSyntaxArray3;
        if (((IEnumerable<ClassDeclarationSyntax>) declarationSyntaxArray2).Any<ClassDeclarationSyntax>())
        {
          foreach (ClassDeclarationSyntax declarationSyntax2 in declarationSyntaxArray2)
          {
            BaseListSyntax baseList = declarationSyntax2.BaseList;
            if ((baseList != null ? baseList.Types.FirstOrDefault<BaseTypeSyntax>((Func<BaseTypeSyntax, bool>) (t => t.ToString().Contains("AbpZeroDbContext"))) : (BaseTypeSyntax) null) != null)
            {
              DbContextDefinition contextDefinition = new DbContextDefinition()
              {
                Namespace = declarationSyntax1.Name.ToString(),
                Name = declarationSyntax2.Identifier.ToString(),
                FullPath = csFile
              };
              contextDefinitionList.Add(contextDefinition);
            }
          }
        }
      }
      return contextDefinitionList;
    }
  }
}
