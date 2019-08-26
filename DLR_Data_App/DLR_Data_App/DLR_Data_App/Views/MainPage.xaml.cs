using DLR_Data_App.Models;
using DLR_Data_App.Views.CurrentProject;
using DLR_Data_App.Views.Login;
using DLR_Data_App.Views.ProjectList;
using DLR_Data_App.Views.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MainPage
  {
    private readonly Dictionary<int, NavigationPage> _menuPages = new Dictionary<int, NavigationPage>();
    public MainPage()
    {
      InitializeComponent();

      MasterBehavior = MasterBehavior.Popover;

      _menuPages.Add((int)MenuItemType.Projects, (NavigationPage)Detail);
    }

    public async Task NavigateFromMenu(int id)
    {
      if (!_menuPages.ContainsKey(id))
      {
        switch (id)
        {
          case (int)MenuItemType.CurrentProject:
            _menuPages.Add(id, new NavigationPage(new ProjectPage()));
            break;
          case (int)MenuItemType.Projects:
            _menuPages.Add(id, new NavigationPage(new ProjectListPage()));
            break;
          case (int)MenuItemType.Sensortest:
            _menuPages.Add(id, new NavigationPage(new SensorTestPage()));
            break;
          case (int)MenuItemType.Settings:
            _menuPages.Add(id, new NavigationPage(new SettingsPage()));
            break;
          case (int)MenuItemType.About:
            _menuPages.Add(id, new NavigationPage(new AboutPage()));
            break;
          case (int)MenuItemType.Logout:
            Application.Current.MainPage = new LoginPage();
            return;
            //break;
        }
      }

      var newPage = _menuPages[id];

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