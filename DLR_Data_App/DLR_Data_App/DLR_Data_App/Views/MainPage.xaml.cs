using DLR_Data_App.Models;
using DLR_Data_App.Services;
using DLR_Data_App.Views.Login;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Localization;
using System.Linq;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage
    {
        //private static Dictionary<MenuItemType, NavigationPage> LoadedMenuPages;
        //private readonly Dictionary<MenuItemType, NavigationPage> _menuPages;
        private SemaphoreSlim NavigationLock = new SemaphoreSlim(1);
        public MenuPage MenuPage { get; }

        [OnSplashScreenLoad]
        static void OnSplashscreenLoad()
        {
            /*
            LoadedMenuPages = new Dictionary<MenuItemType, NavigationPage>
            {
                { MenuItemType.CurrentProject, new NavigationPage(new ProjectPage()) },
                { MenuItemType.Projects, new NavigationPage(new ProjectListPage()) },
                { MenuItemType.ProfilingList, new NavigationPage(new ProfilingListPage()) },
                { MenuItemType.CurrentProfiling, new NavigationPage(new CurrentProfilingPage()) },
                { MenuItemType.Sensortest, new NavigationPage(new SensorTestPage()) },
                { MenuItemType.LowYieldCartograph, new NavigationPage(new DrivingConfigurationSelectionPage()) },
                { MenuItemType.Settings, new NavigationPage(new SettingsPage()) },
                { MenuItemType.About, new NavigationPage(new AboutPage()) }
            };
            */
        }

        public MainPage()
        {
            InitializeComponent();
            //_menuPages = LoadedMenuPages;

            MenuPage = new MenuPage();
            
            Flyout = MenuPage;

            Detail = _menuPages[MenuItemType.LowYieldCartograph];

            FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;

            Appearing += MainPage_Appearing;
        }

        private async void MainPage_Appearing(object sender, EventArgs e)
        {
            Appearing -= MainPage_Appearing;
            await Task.Delay(100);
            MessagingCenter.Send<object, bool>(this, "ReloadToolbar", true);
        }

        /// <summary>
        /// Navigate to selected page
        /// </summary>
        /// <param name="id">Selected page</param>
        /// <returns><see cref="Page"/> which is displayed after this method is finished</returns>
        public async Task<Page> NavigateToPage(Guid id)
        {
            await NavigationLock.WaitAsync();

            if (id == MenuPage.LogoutMenuItem.Id)
            {
                var loginPage = new LoginPage();
                Application.Current.MainPage = loginPage;
                return loginPage;
            }

            var newPage = MenuPage.MenuItems.FirstOrDefault(m => m.Id == id);

            if (newPage != null && Detail != newPage.NavigationPage)
            {
                Detail = newPage.NavigationPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(200);

                MessagingCenter.Send<object, bool>(this, "ReloadToolbar", true);
            }

            IsPresented = false;
            NavigationLock.Release();
            return newPage.NavigationPage;
        }

        DateTime LastBackButtonPress = DateTime.MinValue;

        protected override bool OnBackButtonPressed()
        {
            if (Shell.Current.Navigation.NavigationStack.Count == 1)
            {
                if (!IsPresented)
                {
                    IsPresented = true;
                    return true;
                }
                else
                {
                    if ((DateTime.UtcNow - LastBackButtonPress).TotalSeconds < 3)
                        DependencyService.Get<IAppCloser>().CloseApp();
                    else
                    {
                        LastBackButtonPress = DateTime.UtcNow;
                        DependencyService.Get<IToast>().ShortAlert(SharedResources.appclosewarning);
                    }
                    return true;
                }
            }
            else
            {
                return App.Current.CurrentPage.SendBackButtonPressed();
            }
        }
    }
}