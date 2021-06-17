using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;

namespace DLR_Data_App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SplashScreenPage : ContentPage
	{
        public SplashScreenPage()
		{
			InitializeComponent ();

            Appearing += SplashScreenPage_Appearing;
		}

        private async void SplashScreenPage_Appearing(object sender, EventArgs e)
        {
            Appearing -= SplashScreenPage_Appearing;

            await RunAllMethodsWithAttribute<OnSplashScreenLoadAttribute>();
            await RunAllMethodsWithAttribute<AfterSplashScreenLoadAttribute>();

            Application.Current.MainPage = new MainPage();
        }

        Task RunAllMethodsWithAttribute<T>() where T : Attribute
        {
            //altered from https://stackoverflow.com/a/28791265/8512719#

            var tasksToDo = typeof(SplashScreenPage).Assembly.GetTypes()
                .SelectMany(t => t.GetRuntimeMethods())
                .Where(m => m.IsStatic && m.GetCustomAttribute<T>() != null)
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