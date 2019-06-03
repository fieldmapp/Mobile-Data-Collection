using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DLR_Data_App.Views;
using Login;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DLR_Data_App
{
  public partial class App : Application
  {

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
      
      //MainPage = new NavigationPage(new LoginPage());
      //MainPage = new MainPage();
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
