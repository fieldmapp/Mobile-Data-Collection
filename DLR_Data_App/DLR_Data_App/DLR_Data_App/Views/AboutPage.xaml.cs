using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        /**
         * License Button clicked
         */
        private async void LicenseButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LicensesPage());
        }
    }
}