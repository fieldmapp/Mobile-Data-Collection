using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MenuPage : ContentPage
  {
    MainPage RootPage { get => Application.Current.MainPage as MainPage; }
    List<HomeMenuItem> menuItems;
    public MenuPage()
    {
      InitializeComponent();

      menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Current_Project, Title=AppResources.currentproject },
                new HomeMenuItem {Id = MenuItemType.Projects, Title=AppResources.projects },
                new HomeMenuItem {Id = MenuItemType.Sensortest, Title=AppResources.sensortest },
                new HomeMenuItem {Id = MenuItemType.Settings, Title=AppResources.settings },
                new HomeMenuItem {Id = MenuItemType.About, Title=AppResources.about },
                new HomeMenuItem {Id = MenuItemType.Logout, Title=AppResources.logout }
            };

      ListViewMenu.ItemsSource = menuItems;

      ListViewMenu.SelectedItem = menuItems[0];
      ListViewMenu.ItemSelected += async (sender, e) =>
      {
        if (e.SelectedItem == null)
          return;

        var id = (int)((HomeMenuItem)e.SelectedItem).Id;
        await RootPage.NavigateFromMenu(id);
      };
    }
  }
}