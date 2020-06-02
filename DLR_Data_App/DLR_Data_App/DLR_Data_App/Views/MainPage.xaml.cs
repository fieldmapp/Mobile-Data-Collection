using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using DLR_Data_App.Views.CurrentProject;
using DLR_Data_App.Views.Login;
using DLR_Data_App.Views.ProjectList;
using DLR_Data_App.Views.Settings;
using DLR_Data_App.Views.Profiling;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DLR_Data_App.Views.ProfilingList;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage
    {
        private static Dictionary<MenuItemType, NavigationPage> LoadedMenuPages;
        private readonly Dictionary<MenuItemType, NavigationPage> _menuPages;
        private SemaphoreSlim NavigationLock = new SemaphoreSlim(1);

        [OnSplashScreenLoad]
        static void OnSplashscreenLoad()
        {
            LoadedMenuPages = new Dictionary<MenuItemType, NavigationPage>
            {
                { MenuItemType.CurrentProject, new NavigationPage(new ProjectPage()) },
                { MenuItemType.Projects, new NavigationPage(new ProjectListPage()) },
                { MenuItemType.ProfilingList, new NavigationPage(new ProfilingListPage()) },
                { MenuItemType.CurrentProfiling, new NavigationPage(new CurrentProfilingPage()) },
                { MenuItemType.Sensortest, new NavigationPage(new SensorTestPage()) },
                { MenuItemType.Settings, new NavigationPage(new SettingsPage()) },
                { MenuItemType.About, new NavigationPage(new AboutPage()) },
                { MenuItemType.DistanceMeasuringDemo, new NavigationPage(new DistanceMeasuringDemoPage()) },
                { MenuItemType.VoiceRecognitionDemo, new NavigationPage(new VoiceRecognitionDemoPage()) }
            };
        }

        public MainPage()
        {
            InitializeComponent();
            _menuPages = LoadedMenuPages;

            Detail = _menuPages[MenuItemType.Projects];

            MasterBehavior = MasterBehavior.Popover;
            
            //TODO: check if unsubscribe is needed
            MessagingCenter.Subscribe<EventArgs>(this, "OpenMasterMenu", args => IsPresented = true);

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
        public async Task<Page> NavigateFromMenu(MenuItemType id)
        {
            await NavigationLock.WaitAsync();

            if (id == MenuItemType.Logout)
            {
                var loginPage = new LoginPage();
                Application.Current.MainPage = loginPage;
                return loginPage;
            }

            var newPage = _menuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(200);

                MessagingCenter.Send<object, bool>(this, "ReloadToolbar", true);
            }

            IsPresented = false;
            NavigationLock.Release();
            return newPage;
        }

        DateTime LastBackButtonPress = DateTime.MinValue;

        protected override bool OnBackButtonPressed()
        {
            if ((App.Current as App).Navigation.Navigation.NavigationStack.Count == 1)
            {
                if (!IsPresented)
                {
                    IsPresented = true;
                    return true;
                }
                else
                {
                    if ((DateTime.UtcNow - LastBackButtonPress).TotalSeconds < 3)
                        return base.OnBackButtonPressed();
                    else
                    {
                        LastBackButtonPress = DateTime.UtcNow;
                        DependencyService.Get<IToast>().ShortAlert(AppResources.appclosewarning);
                        return true;
                    }
                }
            }
            else
            {
                return (App.Current as App).CurrentPage.SendBackButtonPressed();
            }
        }
    }
}