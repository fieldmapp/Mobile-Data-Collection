using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Localization;
using DlrDataApp.Modules.OdkProjects.Shared.Views.CurrentProject;
using DlrDataApp.Modules.OdkProjects.Shared.Views.ProjectList;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjects.Shared
{
    class OdkProjectsModule : ModuleBase
    {
        public static OdkProjectsModule Instance;
        public Guid CurrentProjectPageGuid { private set; get; }
        public Guid ProjectListPageGuid { private set; get; }
        public override Task OnInitialize()
        {
            Instance = this;
            ResourcesCollector.AddResource<Localization.OdkProjectsResources>();
            CurrentProjectPageGuid = ModuleHost.AddToSidebar(Localization.OdkProjectsResources.currentproject, new NavigationPage(new ProjectPage()));
            ProjectListPageGuid = ModuleHost.AddToSidebar(Localization.OdkProjectsResources.projects, new NavigationPage(new ProjectListPage()));
            return Task.CompletedTask;
        }
    }
}
