using DLR_Data_App.Models;
using DLR_Data_App.Views.Login;
using DLR_Data_App.Views.Projectlist;
using DLR_Data_App.Views.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MainPage : MasterDetailPage
  {
    Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
    public MainPage()
    {
      InitializeComponent();

      MasterBehavior = MasterBehavior.Popover;

      MenuPages.Add((int)MenuItemType.Projects, (NavigationPage)Detail);
    }

    public async Task NavigateFromMenu(int id)
    {
      if (!MenuPages.ContainsKey(id))
      {
        switch (id)
        {
          case (int)MenuItemType.Current_Project:
            MenuPages.Add(id, new NavigationPage(new ProjectPage()));
            break;
          case (int)MenuItemType.Projects:
            MenuPages.Add(id, new NavigationPage(new ItemsPage()));
            break;
          case (int)MenuItemType.Sensortest:
            MenuPages.Add(id, new NavigationPage(new SensorTestPage()));
            break;
          case (int)MenuItemType.Settings:
            MenuPages.Add(id, new NavigationPage(new SettingsPage()));
            break;
          case (int)MenuItemType.About:
            MenuPages.Add(id, new NavigationPage(new AboutPage()));
            break;
          case (int)MenuItemType.Logout:
            Application.Current.MainPage = new LoginPage();
            return;
            //break;
        }
      }

      var newPage = MenuPages[id];

      if (newPage != null && Detail != newPage)
      {
        Detail = newPage;

        if (Device.RuntimePlatform == Device.Android)
          await Task.Delay(100);

        IsPresented = false;
      }
    }
  }
}