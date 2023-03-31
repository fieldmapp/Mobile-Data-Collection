using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Localization;
using DlrDataApp.Modules.OdkProjects.Shared.Localization;
using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjects.Shared.Views.CurrentProject;
using DlrDataApp.Modules.OdkProjects.Shared.Views.ProjectList;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjects.Shared
{
    public class OdkProjectsModule : ModuleBase<OdkProjectsModule>
    {
        public OdkProjectsModule() : base("Projects", new List<string>() { "Profiling" }) { }
        public override Task OnInitialize()
        {
            // currently working on replacing the akward way in which projects (+ project results) are stored to make it compatible with Database class
            ResourcesCollector.AddResource<OdkProjectsResources>();

            ModuleHost.App.FlyoutItem.Items.Add(new ShellContent
            {
                Title = OdkProjectsResources.currentproject,
                Route = "projectscurrent",
                ContentTemplate = new DataTemplate(typeof(ProjectPage))
            });
            ModuleHost.AddNavigationValidityCheck(path =>
            {
                if (path != "//projectscurrent")
                    return null;

                var currentProfiling = Database.GetActiveElement<Project, ActiveProjectInfo>();
                return currentProfiling == null ? "//projects" : null;
            });
            ModuleHost.App.FlyoutItem.Items.Add(new ShellContent
            {
                Title = OdkProjectsResources.projects,
                Route = "projects",
                ContentTemplate = new DataTemplate(typeof(ProjectListPage))
            });
            return Task.CompletedTask;
        }
    }
}
