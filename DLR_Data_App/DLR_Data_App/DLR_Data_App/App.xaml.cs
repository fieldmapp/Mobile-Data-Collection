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
        public static MainPage CurrentMainPage => Current.MainPage;

        public string FolderLocation { get; } = string.Empty;
        public ThreadSafeRandom RandomProvider { get; } = new ThreadSafeRandom();
        public IUser CurrentUser { get; internal set; }
        public Sensor Sensor { get; }
        public NavigationPage NavigationPage => MainPage?.Detail as NavigationPage;
        public Page CurrentPage => NavigationPage?.CurrentPage;
        public new MainPage MainPage => base.MainPage as MainPage;
        public Database Database { get; }
        public static new App Current => Application.Current as App;

        public IModuleHost ModuleHost { private set; get; }

        List<ISharedModule> Modules;

        /// <summary>
        /// Constructor with database support
        /// </summary>
        /// <param name="folderPath">Path to the location of stored files in the filesystem</param>
        /// <param name="databaseLocation"></param>
        /// <param name="storageProvider">Path to the local database</param>
        public App(string folderPath, string databaseLocation, List<ISharedModule> modules)
        {
            Device.SetFlags(new[] { "Shapes_Experimental" });
            InitializeComponent();

            Modules = modules;
            FolderLocation = folderPath;
            DatabaseLocation = databaseLocation;
            Sensor = new Sensor();
            Database = new Database(databaseLocation);


            base.MainPage = new LoginPage();

            // TODO
            //var a = DependencyService.Get<IUbloxCommunicator>();
        }

        public void AfterSplashScreenLoad()
        {
            ModuleHost = new ModuleHostService(this, MainPage, MainPage.MenuPage, Modules);
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
