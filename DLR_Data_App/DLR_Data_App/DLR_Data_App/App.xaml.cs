using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DLR_Data_App.Views;
using Login;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DLR_Data_App
{
  /*
   * Main class for cross plattform app
   */
  public partial class App : Application
  {
    public static string DatabaseLocation = string.Empty;
    public static string FolderLocation = string.Empty;

    /*
     * Constructor without database support
     */
    public App()
    {
      InitializeComponent();

      if (Preferences.Get("autologin", true))
      {
        MainPage = new MainPage();
      }
      else
      {
        MainPage = new LoginPage();
      }
    }

    /*
     * Constructor with database support
     * @param folderPath Path to the location of stored files in the filesystem
     * @param databaseLocation Path to the local database
     */
    public App(string folderPath, string databaseLocation)
    {
      InitializeComponent();

      FolderLocation = folderPath;
      DatabaseLocation = databaseLocation;
      
      if (Preferences.Get("autologin", true))
      {
        MainPage = new MainPage();
      }
      else
      {
        MainPage = new LoginPage();
      }

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
