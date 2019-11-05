using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System;
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

        DateTime LastBackButtonPress = DateTime.MinValue;

        protected override bool OnBackButtonPressed()
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
}