using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        /**
         * Override hardware back button on Android devices to return to project list
         */
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();
                if ((Application.Current as App).CurrentPage is MainPage mainPage)
                    await mainPage.NavigateFromMenu(1);
            });

            return true;
        }
    }
}