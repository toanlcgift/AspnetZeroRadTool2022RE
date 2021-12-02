using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;

namespace AspNetZeroRadToolVisualStudioExtension.Dialogs.Providers
{
  public static class MultiTenancyChecker
  {
    public static bool IsEnabled(Entity entity)
    {
      try
      {
        return CSharpSyntaxTree.ParseText(File.ReadAllText(entity.RootPath + "\\src\\" + entity.Namespace + ".Core.Shared\\" + entity.ProjectName + "Consts.cs")).GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>().First<FieldDeclarationSyntax>((Func<FieldDeclarationSyntax, bool>) (c => c.ToString().Contains("MultiTenancyEnabled"))).DescendantNodes().OfType<VariableDeclarationSyntax>().First<VariableDeclarationSyntax>().Variables[0].Initializer.Value.ToString().ToLower().Equals("true");
      }
      catch (Exception ex)
      {
        return true;
      }
    }
  }
}
