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
  public static class EntityProvider
  {
    public static string _slnDirectory = "";
    public static string _slnSrcCoreDirectory = "";
    public static List<NavigationProperty> EntityList = new List<NavigationProperty>();
    public static string Namespace = "";

    public static void SetSolutionPathForEntities(string solutionPath) => EntityProvider._slnSrcCoreDirectory = solutionPath;

    public static void SetSolutionDirectory(string solutionPath) => EntityProvider._slnDirectory = solutionPath;

    public static void StoreAllEntitiesDefinitionsInSolution(
      bool clearListBeforeInsert = false,
      string directory = null)
    {
      List<string> filesUnderDirectory = CsFileProvider.GetAllCsFilesUnderDirectory(directory ?? EntityProvider._slnSrcCoreDirectory, new List<string>());
      List<NavigationProperty> source = new List<NavigationProperty>();
      foreach (string csFile in filesUnderDirectory)
      {
        try
        {
          source.AddRange((IEnumerable<NavigationProperty>) EntityProvider.GetEntitiesFromFile(csFile));
        }
        catch (Exception ex)
        {
        }
      }
      List<NavigationProperty> foreignEntities = source;
      int num = 0;
      List<NavigationProperty> derivedEntityList;
      do
      {
        derivedEntityList = new List<NavigationProperty>();
        foreach (string csFile in filesUnderDirectory)
        {
          try
          {
            derivedEntityList = EntityProvider.GetDerivateEntitiesFromFile(csFile, foreignEntities, derivedEntityList);
          }
          catch (Exception ex)
          {
          }
        }
        List<string> stringList = new List<string>();
        for (int index = 0; index < derivedEntityList.Count; ++index)
        {
          NavigationProperty foreignEntity = derivedEntityList[index];
          if (source.Any<NavigationProperty>((Func<NavigationProperty, bool>) (e => e.ForeignEntityName == foreignEntity.ForeignEntityName)))
            stringList.Add(foreignEntity.ForeignEntityName);
        }
        foreach (string str in stringList)
        {
          string item = str;
          derivedEntityList.RemoveAll((Predicate<NavigationProperty>) (e => e.ForeignEntityName == item));
        }
        foreach (NavigationProperty navigationProperty in derivedEntityList)
          source.Add(navigationProperty);
        foreignEntities = derivedEntityList;
        ++num;
      }
      while (derivedEntityList.Count > 0 && num < 500);
      if (clearListBeforeInsert)
        EntityProvider.EntityList = new List<NavigationProperty>();
      EntityProvider.EntityList.AddRange((IEnumerable<NavigationProperty>) source);
      EntityProvider.AddCommonZeroEntities();
    }

    private static List<NavigationProperty> GetEntitiesFromFile(string csFile)
    {
      List<NavigationProperty> navigationPropertyList = new List<NavigationProperty>();
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
            BaseTypeSyntax baseTypeSyntax = baseList != null ? baseList.Types.FirstOrDefault<BaseTypeSyntax>((Func<BaseTypeSyntax, bool>) (t => t.ToString().Contains("Entity"))) : (BaseTypeSyntax) null;
            if (baseTypeSyntax != null)
            {
              NavigationProperty navigationProperty1 = new NavigationProperty()
              {
                Namespace = declarationSyntax1.Name.ToString(),
                ForeignEntityName = declarationSyntax2.Identifier.ToString()
              };
              TypeArgumentListSyntax argumentListSyntax = baseTypeSyntax.DescendantNodes().OfType<TypeArgumentListSyntax>().FirstOrDefault<TypeArgumentListSyntax>();
              if (argumentListSyntax != null)
              {
                SeparatedSyntaxList<TypeSyntax> arguments = argumentListSyntax.Arguments;
                if (arguments.Any())
                {
                  arguments = argumentListSyntax.Arguments;
                  if (!arguments[0].ToString().Contains("<"))
                  {
                    NavigationProperty navigationProperty2 = navigationProperty1;
                    arguments = argumentListSyntax.Arguments;
                    string str = arguments[0].ToString();
                    navigationProperty2.IdType = str;
                  }
                  else
                    continue;
                }
                else
                  continue;
              }
              else
                navigationProperty1.IdType = "int";
              navigationProperty1.DisplayNameList = new List<string>();
              foreach (PropertyDeclarationSyntax declarationSyntax3 in declarationSyntax2.DescendantNodes().OfType<PropertyDeclarationSyntax>())
              {
                if (!(declarationSyntax3.Type.ToString() != "string"))
                  navigationProperty1.DisplayNameList.Add(declarationSyntax3.Identifier.ToString());
              }
              navigationProperty1.FilePath = csFile;
              navigationPropertyList.Add(navigationProperty1);
            }
          }
        }
      }
      return navigationPropertyList;
    }

    private static List<NavigationProperty> GetDerivateEntitiesFromFile(
      string csFile,
      List<NavigationProperty> foreignEntities,
      List<NavigationProperty> derivedEntityList)
    {
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
            NavigationProperty navigationProperty1 = (NavigationProperty) null;
            foreach (NavigationProperty foreignEntity in foreignEntities)
            {
              NavigationProperty baseEntity = foreignEntity;
              BaseListSyntax baseList = declarationSyntax2.BaseList;
              if ((baseList != null ? baseList.Types.FirstOrDefault<BaseTypeSyntax>((Func<BaseTypeSyntax, bool>) (t => t.ToString().Equals(baseEntity.ForeignEntityName))) : (BaseTypeSyntax) null) != null)
              {
                navigationProperty1 = baseEntity;
                break;
              }
            }
            if (navigationProperty1 != null)
            {
              NavigationProperty navigationProperty2 = new NavigationProperty()
              {
                Namespace = declarationSyntax1.Name.ToString(),
                ForeignEntityName = declarationSyntax2.Identifier.ToString(),
                IdType = navigationProperty1.IdType,
                DisplayNameList = new List<string>()
              };
              foreach (PropertyDeclarationSyntax declarationSyntax3 in declarationSyntax2.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                navigationProperty2.DisplayNameList.Add(declarationSyntax3.Identifier.ToString());
              foreach (string displayName in navigationProperty1.DisplayNameList)
                navigationProperty2.DisplayNameList.Add(displayName);
              derivedEntityList.Add(navigationProperty2);
            }
          }
        }
      }
      return derivedEntityList;
    }

    private static void AddCommonZeroEntities()
    {
      List<NavigationProperty> entityList1 = EntityProvider.EntityList;
      if (entityList1 != null)
      {
        // ISSUE: explicit non-virtual call
        entityList1.Add(new NavigationProperty()
        {
          ForeignEntityName = "User",
          IdType = "long",
          DisplayNameList = new List<string>() { "Name" },
          Namespace = EntityProvider.Namespace + ".Authorization.Users"
        });
      }
      List<NavigationProperty> entityList2 = EntityProvider.EntityList;
      if (entityList2 != null)
      {
        // ISSUE: explicit non-virtual call
        entityList2.Add(new NavigationProperty()
        {
          ForeignEntityName = "OrganizationUnit",
          IdType = "long",
          DisplayNameList = new List<string>()
          {
            "DisplayName"
          },
          Namespace = "Abp.Organizations"
        });
      }
      List<NavigationProperty> entityList3 = EntityProvider.EntityList;
      if (entityList3 != null)
      {
        // ISSUE: explicit non-virtual call
        entityList3.Add(new NavigationProperty()
        {
          ForeignEntityName = "ApplicationLanguage",
          IdType = "int",
          DisplayNameList = new List<string>() { "Name" },
          Namespace = "Abp.Localization"
        });
      }
      List<NavigationProperty> entityList4 = EntityProvider.EntityList;
      if (entityList4 == null)
        return;
      // ISSUE: explicit non-virtual call
      entityList4.Add(new NavigationProperty()
      {
        ForeignEntityName = "Role",
        IdType = "int",
        DisplayNameList = new List<string>() { "Name" },
        Namespace = EntityProvider.Namespace + ".Authorization.Roles"
      });
    }
  }
}
