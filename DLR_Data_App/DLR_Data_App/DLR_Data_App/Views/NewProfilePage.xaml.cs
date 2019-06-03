using DLR_Data_App.Services;
using Login;
using Login.Models;
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
  public partial class NewProfilePage : ContentPage
  {
    public NewProfilePage()
    {
      InitializeComponent();

      LoginIcon.HeightRequest = Constants.LoginIconHeight;

      entry_username.Completed += (s, e) => entry_password.Focus();
      entry_password.Completed += (s, e) => btn_accept.Focus();
    }
    
    async void Btn_accept_Clicked(object sender, EventArgs e)
    {
      Preferences.Set("username", entry_username.Text);
      Preferences.Set("password", Helpers.Encrypt_password(entry_password.Text));

      if (Device.RuntimePlatform == Device.Android)
      {
        Application.Current.MainPage = new MainPage();
      }
      else if (Device.RuntimePlatform == Device.iOS)
      {
        await Navigation.PushModalAsync(new MainPage());
      }
    }

    async void Btn_cancel_Clicked(object sender, EventArgs e)
    {
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