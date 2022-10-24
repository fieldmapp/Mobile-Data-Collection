using Xamarin.Forms.Xaml;
using DLR_Data_App.Views.Login;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using Xamarin.Forms;
using System;
using DLR_Data_App.Views;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DLR_Data_App
{
    public partial class App : IApp
    {
        public static string DatabaseLocation = string.Empty;

        public string FolderLocation { get; } = string.Empty;
        public ThreadSafeRandom RandomProvider { get; } = new ThreadSafeRandom();
        public IUser CurrentUser { get; internal set; }
        public Sensor Sensor { get; }
        public Database Database { get; }
        public static new App Current => Application.Current as App;

        public IModuleHost ModuleHost { private set; get; }

        public Page CurrentPage => Shell.Current.CurrentPage;

        public FlyoutItem FlyoutItem => AppShell.Current.ModuleItems;

        List<ISharedModule> Modules;

        /// <summary>
        /// Constructor with database support
        /// </summary>
        /// <param name="folderPath">Path to the location of stored files in the filesystem</param>
        /// <param name="databaseLocation"></param>
        /// <param name="storageProvider">Path to the local database</param>
        public App(string folderPath, string databaseLocation, List<ISharedModule> modules)
        {
            InitializeComponent();

            Modules = modules;
            FolderLocation = folderPath;
            DatabaseLocation = databaseLocation;
            Sensor = new Sensor();
            Database = new Database(databaseLocation);


            base.MainPage = new LoginPage();

        }

        public void AfterSplashScreenLoad()
        {
            ModuleHost = new ModuleHostService(this, Modules, new SharedMethodProvider());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }
        
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }
        
        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
