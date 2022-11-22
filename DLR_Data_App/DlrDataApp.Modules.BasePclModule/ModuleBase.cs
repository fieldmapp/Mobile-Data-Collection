using DlrDataApp.Modules.Base.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    /// <summary>
    /// <para>
    /// Base class for Module implementation. Can be used to provide shorthand access to ModuleHost.App properties.
    /// </para>
    /// <para>
    /// Lifecyle events example:
    /// <list type="number">
    /// <item>
    ///     All Modules are initialized, one after the other
    ///     <list type="number">
    ///     <item> Module1 Initializing</item>
    ///     <item> Module1 Initialized</item>
    ///     <item> Module2 Initializing</item>
    ///     <item> Module2 Initialized</item>
    ///     </list>
    /// </item>
    /// <item>
    ///     All Modules are post-initialized, one after the other
    ///     <list type="number">
    ///     <item> Module1 PostInitializing</item>
    ///     <item> Module1 PostInitialized</item>
    ///     <item> Module2 PostInitializing</item>
    ///     <item> Module2 PostInitialized</item>
    ///     </list>
    /// </item>
    ///     
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of Module, like: <c>TestModule : <see cref="ModuleBase"/>&lt;TestModule&gt;</c></typeparam>
    public abstract class ModuleBase<T> : ISharedModule where T : ModuleBase<T>
    {
        public event EventHandler<IModuleHost> Initializing = delegate { };
        public event EventHandler<IModuleHost> Initialized = delegate { };
        public event EventHandler<IModuleHost> PostInitialized = delegate { };
        public event EventHandler<IModuleHost> PostInitializing = delegate { };

        /// <summary>
        /// Creates a new Instance
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        /// <param name="neededModules">
        /// List of all names of modules which are needed for this module. 
        /// Will get checked for during Initialize
        /// </param>
        public ModuleBase(string moduleName, List<string> neededModules)
        {
            ModuleName = moduleName;
            NeededModules = neededModules;
        }

        /// <summary>
        /// Singleton access to the implementing module
        /// </summary>
        public static T Instance { get; private set; }

        /// <summary>
        /// The modules name
        /// </summary>
        public string ModuleName { get; }

        /// <summary>
        /// The <see cref="IModuleHost"/> provided by the App which is shared between all module
        /// </summary>
        public IModuleHost ModuleHost { get; private set; }

        /// <summary>
        /// Shorthand access to <see cref="ModuleHost.App"/>
        /// </summary>
        public IApp App => ModuleHost.App;

        /// <summary>
        /// Shorthand access to <see cref="ModuleHost.App.CurrentUser"/>
        /// </summary>
        public IUser CurrentUser => ModuleHost.App.CurrentUser;

        /// <summary>
        /// Shorthand access to <see cref="ModuleHost.App.CurrentPage"/>
        /// </summary>
        public Page CurrentPage => ModuleHost.App.CurrentPage;

        /// <summary>
        /// Shorthand access to <see cref="ModuleHost.App.Database"/>
        /// </summary>
        public Database Database => ModuleHost.App.Database;

        /// <summary>
        /// Shorthand access to <see cref="ModuleHost.App.RandomProvider"/>
        /// </summary>
        public ThreadSafeRandom RandomProvider => ModuleHost.App.RandomProvider;

        /// <summary>
        /// Shorthand access to <see cref="ModuleHost.App.Sensor"/>
        /// </summary>
        public Sensor Sensor => ModuleHost.App.Sensor;

        /// <summary>
        /// Shorthand access to <see cref="ModuleHost.App.FolderLocation"/>
        /// </summary>
        public string FolderLocation => ModuleHost.App.FolderLocation;

        /// <summary>
        /// Shorthand access to <see cref="ModuleHost.SharedMethodProvider"/>
        /// </summary>
        public ISharedMethodProvider SharedMethodProvider => ModuleHost.SharedMethodProvider;

        private List<string> NeededModules;

        public async Task Initialize(IModuleHost moduleHost)
        {
            ModuleHost = moduleHost;
            var missingModules = NeededModules.Where(m => !ModuleHost.Modules.Any(m2 => m == m2.ModuleName));
            foreach (var missingModule in missingModules)
                throw new Exception($"Module \"{ModuleName}\" needs Module \"{missingModule}\" which is not loaded");

            Initializing(this, ModuleHost);
            Instance = (T)this;
            await OnInitialize();
            Initialized(this, ModuleHost);
        }

        public virtual Task OnInitialize()
        {
            return Task.CompletedTask;
        }

        public async Task PostInitialize()
        {
            PostInitializing(this, ModuleHost);
            await OnPostInitialize();
            PostInitialized(this, ModuleHost);
        }
        public virtual Task OnPostInitialize()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers a method with the <see cref="SharedMethodProvider"/>
        /// </summary>
        /// <param name="methodName">Public name of the shared method</param>
        /// <param name="method">Func representing the method. You may use tuples to use multiple inputs and outputs.</param>
        protected void RegisterSharedMethod(string methodName, Func<object, object> method)
        {
            SharedMethodProvider.Register(ModuleName, methodName, method);
        }

        /// <summary>
        /// Calls a shared method.
        /// </summary>
        /// <typeparam name="I">Type of input</typeparam>
        /// <typeparam name="R">Type of result</typeparam>
        /// <param name="ModuleIdentifier">Name of module which provides the method to be called</param>
        /// <param name="MethodIdentifier">Public name of method to be called</param>
        /// <param name="input">Input to public method</param>
        /// <returns>Typed result of the shared method</returns>
        public R CallSharedMethod<I, R>(string ModuleIdentifier, string MethodIdentifier, I input)
        {
            return SharedMethodProvider.Call<I, R>(ModuleIdentifier, MethodIdentifier, input);
        }
    }
}
