using DLR_Data_App.Models;
using DLR_Data_App.Views;
using DlrDataApp.Modules.Base.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Services
{
    public class ModuleHostService : IModuleHost
    {
        public List<Func<string, string>> RedirectionFuncs { get; private set; } = new List<Func<string, string>>();

        public List<ISharedModule> Modules { get; }
        public ModuleHostService(IApp app, List<ISharedModule> modules, SharedMethodProvider methodProvider)
        {
            Modules = modules;
            App = app;
            SharedMethodProvider = methodProvider;

            Initialize().Wait();
        }

        private async Task Initialize()
        {
            foreach (var module in Modules)
            {
                await module.Initialize(this);
            }
            foreach (var module in Modules)
            {
                await module.PostInitialize();
            }
        }

        
        public void AddNavigationValidityCheck(Func<string, string> redirectionFunc)
        {
            RedirectionFuncs.Add(redirectionFunc);
        }

        public IApp App { get; }

        public ISharedMethodProvider SharedMethodProvider { get; }
    }
}
