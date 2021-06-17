using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DlrDataApp.Modules.Base.Shared
{
    public abstract class ModuleBase : ISharedModule
    {
        public event EventHandler<IModuleHost> Initializing = delegate { };
        public event EventHandler<IModuleHost> Initialized = delegate { };
        public event EventHandler<IModuleHost> PostInitialized = delegate { };
        public event EventHandler<IModuleHost> PostInitializing = delegate { };
        public IModuleHost ModuleHost { get; private set; }
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
