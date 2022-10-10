using DLR_Data_App.Models;
using DLR_Data_App.Views.Settings;
using DlrDataApp.Modules.Base.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage
    {
        public ObservableCollection<HomeMenuItem> MenuItems { get; } = new ObservableCollection<HomeMenuItem>();
        public HomeMenuItem SensorTestMenuItem { get; }
        public HomeMenuItem SettingsMenuItem { get; }
        public HomeMenuItem AboutMenuItem { get; }
        public HomeMenuItem LogoutMenuItem { get; }

        public MenuPage()
        {
            InitializeComponent();

            MenuItems.Add(SensorTestMenuItem = new HomeMenuItem { Id = Guid.NewGuid(), Title = SharedResources.sensortest, NavigationPage = new NavigationPage(new SensorTestPage()) });
            MenuItems.Add(SettingsMenuItem = new HomeMenuItem { Id = Guid.NewGuid(), Title = SharedResources.settings, NavigationPage = new NavigationPage(new SettingsPage()) });
            MenuItems.Add(AboutMenuItem = new HomeMenuItem { Id = Guid.NewGuid(), Title = SharedResources.about, NavigationPage = new NavigationPage(new AboutPage()) });
            MenuItems.Add(LogoutMenuItem = new HomeMenuItem { Id = Guid.NewGuid(), Title = SharedResources.logout });

            /* = new ObservableCollection<HomeMenuItem>
            {
                SensorTestMenuItem,
                SettingsMenuItem,
                AboutMenuItem,
                LogoutMenuItem
                // TODO
                //new HomeMenuItem { Id = MenuItemType.CurrentProject, Title=SharedResources.currentproject },
                //new HomeMenuItem { Id = MenuItemType.Projects, Title=SharedResources.projects },
                //new HomeMenuItem { Id = MenuItemType.CurrentProfiling, Title = SharedResources.currentprofiling },
                //new HomeMenuItem { Id = MenuItemType.ProfilingList, Title = SharedResources.profiling },
                //new HomeMenuItem { Id = MenuItemType.DistanceMeasuringDemo, Title=SharedResources.movement },
                //new HomeMenuItem { Id = MenuItemType.VoiceRecognitionDemo, Title=SharedResources.voicerecognition },
                //new HomeMenuItem { Id = MenuItemType.DrivingEasy, Title=SharedResources.drivingvieweasy },
                //new HomeMenuItem { Id = MenuItemType.DrivingHard, Title=SharedResources.drivingviewhard }
            };*/

            ListViewMenu.ItemsSource = MenuItems;

            //ListViewMenu.ItemTapped += async (sender, e) =>
            //{
            //    if (e.Item == null)
            //        return;
            //
            //    var id = ((HomeMenuItem)e.Item).Id;
            //    await App.CurrentMainPage.NavigateToPage(id);
            //};
        }
    }
}