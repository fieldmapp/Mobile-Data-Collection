using DLR_Data_App.Views.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            SetTabBarIsVisible(this, false);
            BindingContext = this;
            Navigating += AppShell_Navigating;
            //Items.Add(new FlyoutItem { Title = "Test", Items = { new ShellContent { Route = "test", ContentTemplate = new DataTemplate(typeof(Page)) } } });
        }

        private void AppShell_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.Target.Location.OriginalString == "//logout")
            {
                e.Cancel();
                App.Current.MainPage = new LoginPage();
            }
        }
    }
}