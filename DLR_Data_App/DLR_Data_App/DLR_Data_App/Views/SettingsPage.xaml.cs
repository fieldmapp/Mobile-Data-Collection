using DLR_Data_App.Localizations;
using Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class SettingsPage : ContentPage
  {
    public SettingsPage()
    {
      InitializeComponent();
      init();
    }

    private void init()
    {
      entry_name.Text = Preferences.Get("username", "");
      entry_password.Text = "";

      switch_autologin.IsToggled = Preferences.Get("autologin", true);

      switch_accelerometer.IsToggled = Preferences.Get("accelerometer", true);
      switch_barometer.IsToggled = Preferences.Get("barometer", true);
      switch_compass.IsToggled = Preferences.Get("compass", true);
      switch_gps.IsToggled = Preferences.Get("gps", true);
      switch_gyroscope.IsToggled = Preferences.Get("gyroscope", true);
      switch_magnetometer.IsToggled = Preferences.Get("magnetometer", true);
    }

    async void Btn_remove_account_Clicked(object sender, EventArgs e)
    {
      bool answer = await DisplayAlert(AppResources.removeaccount, AppResources.removeaccountwarning, AppResources.accept, AppResources.decline);

      if (answer)
      {
        // remove profile
        Remove_Profile();
      }
    }

    private void Switch_accelerometer_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("accelerometer", switch_accelerometer.IsToggled);
    }

    private void Switch_barometer_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("barometer", switch_barometer.IsToggled);
    }

    private void Switch_compass_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("compass", switch_compass.IsToggled);
    }

    private void Switch_gps_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("gps", switch_gps.IsToggled);
    }

    private void Switch_gyroscope_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("gyroscope", switch_gyroscope.IsToggled);
    }

    private void Switch_magnetometer_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("magnetometer", switch_magnetometer.IsToggled);
    }

    async void Switch_privacy_policy_Toggled(object sender, ToggledEventArgs e)
    {
      bool answer = await DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.accept, AppResources.decline);

      if (answer)
      {
        switch_privacy_policy.IsToggled = true;
      } else
      {
        // remove profile
        Remove_Profile();
      }
    }

    private void Switch_autologin_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("autologin", switch_autologin.IsToggled);
    }

    private async void Remove_Profile()
    {
      Preferences.Set("username", "");
      Preferences.Set("password", "");

      if (Device.RuntimePlatform == Device.Android)
      {
        Application.Current.MainPage = new LoginPage();
      }
      else if (Device.RuntimePlatform == Device.iOS)
      {
        await Navigation.PushModalAsync(new LoginPage());
      }
    }
  }
}