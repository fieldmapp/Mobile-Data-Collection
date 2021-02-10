using DlrDataApp.Modules.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DlrDataApp.Modules.OdkProjectsSharedModule
{
    class OdkProjectsSharedModule : BaseSharedModule
    {
        public static OdkProjectsSharedModule Instance;
        public override Task OnInitialize()
        {
            Instance = this;
            SharedModule.Localization.ResourcesCollector.AddResource<Localization.AppResources>();
            return Task.CompletedTask;
        }
    }
}
