using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage
    {
        public MenuPage()
        {
            InitializeComponent();

            var menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.CurrentProject, Title=AppResources.currentproject },
                new HomeMenuItem {Id = MenuItemType.Projects, Title=AppResources.projects },
                new HomeMenuItem {Id = MenuItemType.Sensortest, Title=AppResources.sensortest },
                new HomeMenuItem {Id = MenuItemType.Settings, Title=AppResources.settings },
                new HomeMenuItem {Id = MenuItemType.About, Title=AppResources.about },
                new HomeMenuItem {Id = MenuItemType.Logout, Title=AppResources.logout },
                new HomeMenuItem {Id = MenuItemType.Survey, Title = AppResources.survey }
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = ((HomeMenuItem)e.SelectedItem).Id;
                await ((Application.Current as App).Navigation.RootPage as MainPage).NavigateFromMenu(id);
            };
        }
    }
}