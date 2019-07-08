using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using DLR_Data_App.Views;
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
namespace DLR_Data_App.Views.Login
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class LoginPage : ContentPage
  {
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
      LoginIcon.HeightRequest = 120;

      // after completing the input of username change focus to password and after that to login button
      entry_Username.Completed += (s, e) => entry_Password.Focus();
      entry_Password.Completed += (s, e) => btn_signin.Focus();

    }

    /**
     * Get login data, check them and on success navigate to project list
     */
    async void Btn_signin_Clicked(object sender, EventArgs e)
    {
      if (Check_Information())
      {
        bool answer = await DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.accept, AppResources.decline);

        if(answer)
        {
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

    /**
     * Open form for creating a new user
     */
    async void Btn_newaccount_Clicked(object sender, EventArgs e)
    {
      //bool answer = await DisplayAlert(AppResources.newaccount, AppResources.newaccountwarning, AppResources.accept, AppResources.decline);

      //if (answer)
      //{
        if (Device.RuntimePlatform == Device.Android)
        {
          Application.Current.MainPage = new NewProfilePage();
        }
        else if (Device.RuntimePlatform == Device.iOS)
        {
          await Navigation.PushModalAsync(new NewProfilePage());
        }
      //}
    }

    /**
     * Checks login with stored data
     */
    private bool Check_Information()
    {
      // Get all users from database
      List<User> userList = Database.ReadUser();

      // Get input from user
      string check_username = entry_Username.Text;
      string check_password = Helpers.Encrypt_password(entry_Password.Text);

      // Check if input empty
      if (check_username == "" 
        || check_password == "")
      {
        return false;
      }

      // Check if entry match with user in database
      foreach(User user in userList)
      {
        // If user is found set as current user
        if(user.Username == check_username 
          && user.Password == check_password)
        {
          App.CurrentUser = user;
          return true;
        }
      }

      return false;
    }
  }
}