using AspNetZeroRadToolVisualStudioExtension.Dialogs;
using AspNetZeroRadToolVisualStudioExtension.Dialogs.EntityLoaders;
using AspNetZeroRadToolVisualStudioExtension.Dialogs.Providers;
using AspNetZeroRadToolVisualStudioExtension.EntityInfo;
using AspNetZeroRadToolVisualStudioExtension.Helpers;
using EnvDTE;
using log4net;
using log4net.Config;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace AspNetZeroRadToolVisualStudioExtension
{
    internal sealed class AspNetZeroRadTool
    {
        public const int CreateNewCommandId = 4368;
        public const int RegenerateCommandId = 4369;
        public const int DatabaseCommandId = 4370;
        public const int AboutCommandId = 4371;
        public static readonly Guid CommandSet = new Guid("adec5afc-2a99-42c7-80bb-477e48be6338");
        private readonly Package _package;
        private readonly ILog _logger;
        private bool _isAboutFormOpen;

        private AspNetZeroRadTool(Package package)
        {
            this._logger = LogManager.GetLogger(typeof(AspNetZeroRadTool));
            AspNetZeroRadTool.ConfigureLog4Net();
            this._package = package ?? throw new ArgumentNullException(nameof(package));
            if (!(this.ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService service))
                return;
            MenuCommand command1 = new MenuCommand(new EventHandler(this.CreateNewEntity), new CommandID(AspNetZeroRadTool.CommandSet, 4368));
            ((MenuCommandService)service).AddCommand(command1);
            MenuCommand command2 = new MenuCommand(new EventHandler(this.RegenerateEntity), new CommandID(AspNetZeroRadTool.CommandSet, 4369));
            ((MenuCommandService)service).AddCommand(command2);
            MenuCommand command3 = new MenuCommand(new EventHandler(this.DatabaseEntity), new CommandID(AspNetZeroRadTool.CommandSet, 4370));
            ((MenuCommandService)service).AddCommand(command3);
            MenuCommand command4 = new MenuCommand(new EventHandler(this.AboutForm), new CommandID(AspNetZeroRadTool.CommandSet, 4371));
            ((MenuCommandService)service).AddCommand(command4);
        }

        private static void ConfigureLog4Net()
        {
            try
            {
                string installationDirectoryOrNull = AppSettings.GetExtensionInstallationDirectoryOrNull();
                if (installationDirectoryOrNull == null)
                    return;
                XmlConfigurator.Configure(new FileInfo(Path.Combine(installationDirectoryOrNull, "log4net.config")));
            }
            catch (Exception ex)
            {
                MsgBox.Exception(ex.Message, ex);
            }
        }

        public static AspNetZeroRadTool Instance { get; private set; }

        private IServiceProvider ServiceProvider => (IServiceProvider)this._package;

        public static void Initialize(Package package)
        {
            Logger.Info("Initializing...");
            Application.ThreadException += new ThreadExceptionEventHandler(ExceptionHandler.HandleThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler.HandleUnhandledException);
            AspNetZeroRadTool.Instance = new AspNetZeroRadTool(package);
        }

        private void CreateNewEntity(object sender, EventArgs e) => this.MenuItemCallback(false, false);

        private void RegenerateEntity(object sender, EventArgs e) => this.MenuItemCallback(true, false);

        private void DatabaseEntity(object sender, EventArgs e) => this.MenuItemCallback(false, true);

        private void AboutForm(object sender, EventArgs e) => this.MenuItemCallback(false, false, true);

        private void MenuItemCallback(bool loadFromJson, bool loadFromDatabase, bool showAboutForm = false)
        {
            this._logger.Debug((object)("Menu item clicked with params > loadFromJson: " + loadFromJson.ToString() + ", loadFromDatabase: " + loadFromDatabase.ToString() + ", showAboutForm: " + showAboutForm.ToString()));
            if (showAboutForm)
            {
                this.ShowAboutFormIfNotShown();
            }
            else
            {
                DTE service = (DTE)this.ServiceProvider.GetService(typeof(DTE));
                string solutionFullName;
                Entity entity;
                try
                {
                    solutionFullName = this.GetSolutionFullName(service);
                    EntityProvider.SetSolutionDirectory(Path.GetDirectoryName(solutionFullName));
                    entity = this.DetectInformationOfProject(solutionFullName);
                    EnumProvider.SetSolutionPathForEnums(Path.GetDirectoryName(solutionFullName) + "\\src");
                    EntityProvider.SetSolutionPathForEntities(Path.GetDirectoryName(solutionFullName) + "\\src\\" + entity.Namespace + ".Core");
                    DbContextProvider.SetSolutionPathForDbContexts(Path.GetDirectoryName(solutionFullName) + "\\src\\" + entity.Namespace + ".EntityFrameworkCore");
                }
                catch (Exception ex)
                {
                    MsgBox.Exception("Couldn't load information of your ASP.NET Zero project!", ex);
                    return;
                }
                try
                {
                    if (loadFromJson)
                    {
                        using (JsonEntityForm jsonEntityForm = new JsonEntityForm(Path.GetDirectoryName(solutionFullName)))
                        {
                            if (jsonEntityForm.ShowDialog() != DialogResult.OK)
                                return;
                            entity = EntityLoaderFromJson.Build(File.ReadAllText(Path.GetDirectoryName(solutionFullName) + "\\AspNetZeroRadTool\\" + jsonEntityForm.SelectedFile + ".json"), entity);
                        }
                    }
                    else if (loadFromDatabase)
                    {
                        using (DbTableSelectionForm tableSelectionForm = new DbTableSelectionForm(Path.GetDirectoryName(solutionFullName), entity))
                        {
                            if (tableSelectionForm.ShowDialog() != DialogResult.OK)
                                return;
                            entity = tableSelectionForm.SelectedTable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.Exception("Couldn't load information of the entity!", ex);
                    return;
                }
                try
                {
                    new EntityGeneratorForm(entity).Show();
                }
                catch (Exception ex)
                {
                    MsgBox.Exception("Encountered a fatal error!", ex);
                }
            }
        }

        private void ShowAboutFormIfNotShown()
        {
            if (this._isAboutFormOpen)
                return;
            new AspNetZeroRadToolVisualStudioExtension.Dialogs.AboutForm((Action)(() => this._isAboutFormOpen = false)).Show();
            this._isAboutFormOpen = true;
        }

        public string GetSolutionFullName(DTE dte)
        {
            try
            {
                return ((_Solution)((_DTE)dte).Solution).FullName;
            }
            catch (Exception ex)
            {
                throw new Exception("Solution information not found!");
            }
        }

        public Entity DetectInformationOfProject(string slnFile)
        {
            Entity entity = new Entity();
            string directoryName = Path.GetDirectoryName(slnFile);
            JObject jobject = File.Exists(directoryName + "\\AspNetZeroRadTool\\config.json") ? JObject.Parse(File.ReadAllText(directoryName + "\\AspNetZeroRadTool\\config.json")) : throw new Exception("Config file not found!");
            entity.ProjectName = (string)jobject["ProjectName"];
            entity.CompanyName = (string)jobject["CompanyName"];
            entity.ProjectType = (string)jobject["ProjectType"];
            entity.ProjectVersion = (string)(jobject["ProjectVersion"] ?? (JToken)"v1.0.0");
            EntityProvider.Namespace = entity.Namespace;
            entity.AppAreaName = entity.ProjectType.Equals("Mvc") ? (string)jobject["ApplicationAreaName"] : "NotNeeded";
            entity.RootPath = directoryName;
            entity.EntityHistoryDisabled = AspNetZeroRadTool.ProjectVersionToNumber(entity.ProjectVersion) < 60300;
            entity.Properties = new List<EntityInfo.Property>();
            entity.PagePermission = new PagePermissionInfo();
            entity.EnumDefinitions = new List<EnumDefinition>();
            entity.NavigationProperties = new List<NavigationProperty>();
            return entity;
        }

        private static int ProjectVersionToNumber(string version)
        {
            if (version.Contains("rc"))
                version = version.Substring(0, version.IndexOf('-'));
            string[] strArray = version.Replace("v", "").Split('.');
            string str1 = "";
            if (strArray.Length == 1)
                strArray = new string[3] { strArray[0], "0", "0" };
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
                return 50500;
            }
        }
    }
}
