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
    public class ProfilingModule : ModuleBase
    {
        public static ProfilingModule Instance;
        public override Task OnInitialize()
        {
            ResourcesCollector.AddResource<ProfilingResources>();
            Instance = this;
            var a = new[] {
                new KeyValuePair<string, NavigationPage>(ProfilingResources.currentprofiling, new NavigationPage(new CurrentProfilingPage())),
                new KeyValuePair<string, NavigationPage>(ProfilingResources.profiling, new NavigationPage(new ProfilingListPage())) };
            var b = ModuleHost.AddSidebarItems(a);
            return Task.CompletedTask;
        }
    }
}
