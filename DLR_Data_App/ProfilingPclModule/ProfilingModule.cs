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
using DlrDataApp.Modules.Profiling.Shared.Models;

namespace DlrDataApp.Modules.Profiling.Shared
{
    public class ProfilingModule : ModuleBase<ProfilingModule>
    {
        public Guid CurrentProfilingPageGuid { private set; get; }
        public Guid ProfilingListPageGuid { private set; get; }
        public override Task OnInitialize()
        {
            ResourcesCollector.AddResource<ProfilingResources>();
            ModuleHost.App.FlyoutItem.Items.Add(new ShellContent
            {
                Title = ProfilingResources.currentprofiling,
                Route = "profilingcurrent",
                ContentTemplate = new DataTemplate(typeof(CurrentProfilingPage))
            });
            ModuleHost.AddNavigationValidityCheck(path =>
            {
                if (path != "//profilingcurrent")
                    return null;

                var currentProfiling = Database.FindWithChildren<ActiveProfilingInfo>(t => true, true)?.ActiveProfiling;
                return currentProfiling == null ? "//profiling" : null;
            });


            ModuleHost.App.FlyoutItem.Items.Add(new ShellContent
            {
                Title = ProfilingResources.profiling,
                Route = "profiling",
                ContentTemplate = new DataTemplate(typeof(ProfilingListPage))
            });

            return Task.CompletedTask;
        }
    }
}
