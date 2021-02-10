using DlrDataApp.Modules.SharedModule;
using DlrDataApp.Modules.SharedModule.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.ProfilingSharedModule
{
    public class ProfilingSharedModule : BaseSharedModule
    {
        public static ProfilingSharedModule Instance;
        public override Task OnInitialize()
        {
            ResourcesCollector.AddResource<Localization.AppResources>();
            Instance = this;
            return Task.CompletedTask;
        }
    }
}
