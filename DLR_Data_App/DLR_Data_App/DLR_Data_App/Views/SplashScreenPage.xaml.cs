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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DLR_Data_App.Views.ProfilingList;
using System.Runtime.CompilerServices;
using System.Reflection;

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

            var builderTask = CreateNeededRessources();
            var displayPrivacyPolicyTask = ShouldDisplayPrivacyPolicy ? DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.accept, AppResources.decline) : Task.FromResult(true);

            var answer = await displayPrivacyPolicyTask;
            if (!answer)
            {
                Application.Current.MainPage = new LoginPage();
                return;
            }
            await builderTask;
            Application.Current.MainPage = new MainPage();
        }

        Task CreateNeededRessources()
        {
            //altered from https://stackoverflow.com/a/28791265/8512719#

            var tasksToDo = typeof(SplashScreenPage).Assembly.GetTypes()
                .SelectMany(t => t.GetRuntimeMethods())
                .Where(m => m.IsStatic && m.GetCustomAttribute<OnSplashScreenLoadAttribute>() != null)
                .Select(m => new Task(() => m.Invoke(null, null)))
                .ToList();

            foreach (var task in tasksToDo)
            {
                task.Start();
            }

            return Task.WhenAll(tasksToDo);
        }
    }
}