using DlrDataApp.Modules.Base.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    public abstract class ModuleBase : ISharedModule
    {
        public event EventHandler<IModuleHost> Initializing = delegate { };
        public event EventHandler<IModuleHost> Initialized = delegate { };
        public event EventHandler<IModuleHost> PostInitialized = delegate { };
        public event EventHandler<IModuleHost> PostInitializing = delegate { };
        public IModuleHost ModuleHost { get; private set; }

        public IApp App => ModuleHost.App;
        public IUser CurrentUser => ModuleHost.App.CurrentUser;
        public Page CurrentPage => ModuleHost.App.CurrentPage;
        public Database Database => ModuleHost.App.Database;
        public ThreadSafeRandom RandomProvider => ModuleHost.App.RandomProvider;
        public Sensor Sensor => ModuleHost.App.Sensor;
        public string FolderLocation => ModuleHost.App.FolderLocation;

        public async Task Initialize(IModuleHost moduleHost)
        {
            ModuleHost = moduleHost;
            Initializing(this, ModuleHost);
            await OnInitialize();
            Initialized(this, ModuleHost);
        }

        public virtual Task OnInitialize() => Task.CompletedTask;

        public async Task PostInitialize()
        {
            PostInitializing(this, ModuleHost);
            await OnPostInitialize();
            PostInitialized(this, ModuleHost);
        }
        public virtual Task OnPostInitialize() => Task.CompletedTask;
    }
}
