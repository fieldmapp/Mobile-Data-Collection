using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using DLR_Data_App.Views;
using Login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/**
 * Login page class 
 */
namespace Login
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class LoginPage : ContentPage
  {
    private User user;

    public LoginPage()
    {
      InitializeComponent();
      Init();
    }

    /**
     * Initialize page
     */
    private void Init()
    {
      BackgroundColor = Constants.BackgroundColor;
      //lbl_Username.TextColor = Constants.MainTextColor;
      //lbl_Password.TextColor = Constants.MainTextColor;
      LoginIcon.HeightRequest = Constants.LoginIconHeight;

      // after completing the input of username change focus to password and after that to login button
      entry_Username.Completed += (s, e) => entry_Password.Focus();
      entry_Password.Completed += (s, e) => btn_signin.Focus();

      Preferences.Set("autologin", false);
    }

    /**
     * Method checks user login
     */
    async void Btn_signin_Clicked(object sender, EventArgs e)
    {
      this.user = new User(entry_Username.Text, entry_Password.Text);
      if (Check_Information())
      {
        bool answer = await DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.accept, AppResources.decline);

        if(answer)
        {
          Preferences.Set("autologin", switch_rememberlogin.IsToggled);

          if (Device.RuntimePlatform == Device.Android)
          {
            Application.Current.MainPage = new MainPage();
          }
          else if (Device.RuntimePlatform == Device.iOS)
          {
            await Navigation.PushModalAsync(new MainPage());
          }
        }
      }
      else
      {
        await DisplayAlert(AppResources.login, AppResources.failed, AppResources.back);
      }
    }

    async void Btn_newaccount_Clicked(object sender, EventArgs e)
    {
      bool answer = await DisplayAlert(AppResources.newaccount, AppResources.newaccountwarning, AppResources.accept, AppResources.decline);

      if (answer)
      {
        if (Device.RuntimePlatform == Device.Android)
        {
          Application.Current.MainPage = new NewProfilePage();
        }
        else if (Device.RuntimePlatform == Device.iOS)
        {
          await Navigation.PushModalAsync(new NewProfilePage());
        }
      }
    }

    /*
     * Checks login with stored data
     */
    private bool Check_Information()
    {
      string username = Preferences.Get("username", "");
      string password = Preferences.Get("password", "");

      string check_username = entry_Username.Text;
      string check_password = Helpers.Encrypt_password(entry_Password.Text);

      if (username == "" || password == "")
      {
        return false;
      }

      if (username == check_username && password == check_password)
      {
        return true;
      }

      return false;
    }
  }
}