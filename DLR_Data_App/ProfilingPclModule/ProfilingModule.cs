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
using DLR_Data_App.Services;

namespace DlrDataApp.Modules.Profiling.Shared
{
    public class ProfilingModule : ModuleBase<ProfilingModule>
    {
        public ProfilingModule() : base("Profiling", new List<string> { }) { }

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

                var currentProfiling = Database.GetActiveElement<ProfilingData, ActiveProfilingInfo>();
                return currentProfiling == null ? "//profiling" : null;
            });
            RegisterSharedMethod("IsProfilingLoaded", (object input) =>
            {
                if (!(input is string profilingName))
                    return false;
                return ProfilingStorageManager.IsProfilingModuleLoaded(profilingName);
            });
            RegisterSharedMethod("RecentProfilingFinished", (object input) =>
            {
                if (!(input is string profilingName))
                    return false;
                const int MaxDaysSinceLastProfilingCompletion = 45;
                const int MaxProjectsFilledPerProfiling = 10;

                var lastAnsweredProfilingDate = ProfilingStorageManager.GetLastCompletedProfilingDate(profilingName);
                if ((DateTime.UtcNow - lastAnsweredProfilingDate).TotalDays > MaxDaysSinceLastProfilingCompletion
                    || ProfilingStorageManager.ProjectsFilledSinceLastProfilingCompletion > MaxProjectsFilledPerProfiling)
                {
                    return false;
                }
                return true;
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
