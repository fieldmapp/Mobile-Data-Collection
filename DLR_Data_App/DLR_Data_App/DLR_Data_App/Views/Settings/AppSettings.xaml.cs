using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using DLR_Data_App.Views.Login;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DLR_Data_App.Services;

namespace DLR_Data_App.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppSettings
    {
        private readonly List<string> _elementList;
        
        public AppSettings ()
        {
            InitializeComponent ();
            _elementList = new List<string> {AppResources.privacypolicy, AppResources.removedatabase, AppResources.exportdatabase};
            
            AppSettingsList.ItemsSource = _elementList;
        }
        
        /**
         * Update app list on each appearance
         */
        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            AppSettingsList.ItemsSource = _elementList;
        }
        
        /**
         * List of settings about the app shown
         */
        private async void AppSettingsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            bool answer;
            switch (e.ItemIndex)
            {
                case 0:
                    // Show privacy policy
                    answer = await DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.accept, AppResources.decline);
                    if (!answer)
                    {
                        // if user declines privacy policy remove his account
                    }
                    break;
                case 1:
                    // Remove Database
                    answer = await DisplayAlert(AppResources.removedatabase, AppResources.removedatabasewarning, AppResources.accept, AppResources.cancel);
                    if(answer)
                    {
                        if(Database.RemoveDatabase())
                        {
                            // Successful
                            await DisplayAlert(AppResources.removedatabase, AppResources.successful, AppResources.okay);
                            App.Current.MainPage = new LoginPage();
                        }
                        else
                        {
                            // Failure
                            await DisplayAlert(AppResources.removedatabase, AppResources.failed, AppResources.cancel);
                        }
                    }
                    break;
                case 2:
                    // Export Database
                    var exportString = Helpers.ExportData();
                    (Application.Current as App).StorageProvider.ExportDatabase(exportString);
                    await DisplayAlert(AppResources.save, AppResources.successful, AppResources.okay);
                    break;
            }
        }
    }
}