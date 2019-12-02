using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using DLR_Data_App.Views.CurrentProject;
using DLR_Data_App.Views.Login;
using DLR_Data_App.Views.ProjectList;
using DLR_Data_App.Views.Settings;
using DLR_Data_App.Views.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SplashScreenPage : ContentPage
	{
        Dictionary<MenuItemType, NavigationPage> MenuItems;
        bool ShouldDisplayPrivacyPolicy;

        public SplashScreenPage(bool shouldDisplayPrivacyPolicy)
		{
            ShouldDisplayPrivacyPolicy = shouldDisplayPrivacyPolicy;
			InitializeComponent ();

            Appearing += SplashScreenPage_Appearing;
		}

        private async void SplashScreenPage_Appearing(object sender, EventArgs e)
        {
            Appearing -= SplashScreenPage_Appearing;

            var builderTask = Task.Run(() => CreateNeededRessources());
            var displayPrivacyPolicyTask = ShouldDisplayPrivacyPolicy ? DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.accept, AppResources.decline) : Task.FromResult(true);

            var answer = await displayPrivacyPolicyTask;
            if (!answer)
            {
                Application.Current.MainPage = new LoginPage();
                return;
            }
            await builderTask;
            Application.Current.MainPage = new MainPage(MenuItems);
        }

        void CreateNeededRessources()
        {
            Sensor.Instance = new Sensor();

            MenuItems = new Dictionary<MenuItemType, NavigationPage>
            {
                { MenuItemType.CurrentProject, new NavigationPage(new ProjectPage()) },
                { MenuItemType.Projects, new NavigationPage(new ProjectListPage()) },
                { MenuItemType.Profiling, new NavigationPage(new SurveyListPage()) },
                { MenuItemType.Sensortest, new NavigationPage(new SensorTestPage()) },
                { MenuItemType.Settings, new NavigationPage(new SettingsPage()) },
                { MenuItemType.About, new NavigationPage(new AboutPage()) }
            };
        }
    }
}