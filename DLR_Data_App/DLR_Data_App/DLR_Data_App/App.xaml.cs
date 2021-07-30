using Xamarin.Forms.Xaml;
using DLR_Data_App.Views.Login;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using Xamarin.Forms;
using System;
using DLR_Data_App.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DLR_Data_App
{
    public partial class App
    {
        public static string DatabaseLocation = string.Empty;
        public static string FolderLocation = string.Empty;
        public static ThreadSafeRandom RandomProvider = new ThreadSafeRandom();
        public static Views.MainPage CurrentMainPage => (Current.MainPage as Views.MainPage);
        public static User CurrentUser;
        public NavigationPage Navigation => (MainPage as FlyoutPage)?.Detail as NavigationPage;
        public IStorageProvider StorageProvider;
        public Page CurrentPage => Navigation?.CurrentPage;
        public new MainPage MainPage => base.MainPage as MainPage;
        
        /// <summary>
        /// Constructor with database support
        /// </summary>
        /// <param name="folderPath">Path to the location of stored files in the filesystem</param>
        /// <param name="databaseLocation"></param>
        /// <param name="storageProvider">Path to the local database</param>
        public App(string folderPath, string databaseLocation, IStorageProvider storageProvider)
        {
            InitializeComponent();
            
            StorageProvider = storageProvider;
            
            FolderLocation = folderPath;
            DatabaseLocation = databaseLocation;
            
            base.MainPage = new LoginPage();

            var ubloxCommService = DependencyService.Get<IUbloxCommunicator>();
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
