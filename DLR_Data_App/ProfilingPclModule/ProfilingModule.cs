using DlrDataApp.Modules.Profiling.Shared.Localization;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using DlrDataApp.Modules.Profiling.Shared.Views;
using DlrDataApp.Modules.Profiling.Shared.Views.ProfilingList;

namespace DlrDataApp.Modules.Profiling.Shared
{
    public class ProfilingModule : ModuleBase<ProfilingModule>
    {
        public Guid CurrentProfilingPageGuid { private set; get; }
        public Guid ProfilingListPageGuid { private set; get; }
        public override Task OnInitialize()
        {
            ResourcesCollector.AddResource<ProfilingResources>();
            CurrentProfilingPageGuid = ModuleHost.AddToSidebar(ProfilingResources.currentprofiling, new NavigationPage(new CurrentProfilingPage()));
            ProfilingListPageGuid = ModuleHost.AddToSidebar(ProfilingResources.profiling, new NavigationPage(new ProfilingListPage()));

            return Task.CompletedTask;
        }
    }
}
