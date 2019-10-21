using Xamarin.Forms.Xaml;
using DLR_Data_App.Views.Login;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using Xamarin.Forms;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DLR_Data_App
{
    /*
     * Main class for cross platform app
     */
    public partial class App
    {
        public static string DatabaseLocation = string.Empty;
        public static string FolderLocation = string.Empty;
        public static User CurrentUser;
        public NavigationPage Navigation => MainPage as NavigationPage;
        public IStorageProvider StorageProvider;
        public Page CurrentPage => Navigation.CurrentPage;
        
        /*
         * Constructor with database support
         * @param folderPath Path to the location of stored files in the filesystem
         * @param databaseLocation Path to the local database
         */
        public App(string folderPath, string databaseLocation, IStorageProvider storageProvider)
        {
            InitializeComponent();

            DatabankCommunication.Initilize(storageProvider);
            StorageProvider = storageProvider;
            
            FolderLocation = folderPath;
            DatabaseLocation = databaseLocation;
            
            MainPage = new NavigationPage(new LoginPage());
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
