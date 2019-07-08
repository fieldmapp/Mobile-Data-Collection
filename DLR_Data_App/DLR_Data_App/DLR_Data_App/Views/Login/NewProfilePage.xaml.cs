using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Login
{
  /**
   * Creating a new profile, same design like login page
   */
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class NewProfilePage : ContentPage
  {
    private User user;

    /**
     * Constructor
     */
    public NewProfilePage()
    {
      InitializeComponent();

      // setting icon size
      LoginIcon.HeightRequest = 120;

      // directing user to next entry after inserting text to make it easier to insert informations
      entry_username.Completed += (s, e) => entry_password.Focus();
      entry_password.Completed += (s, e) => btn_accept.Focus();
    }
    
    /**
     * Adding user
     */
    async void Btn_accept_Clicked(object sender, EventArgs e)
    {
      bool answer = await DisplayAlert(AppResources.privacypolicy, AppResources.privacytext1, AppResources.accept, AppResources.decline);

      if (!answer)
      {
        // if user dont accept privacy policy cancel process and return to login page
        await Cancel();
      }
      
      user = new User();
      user.Username = entry_username.Text;
      user.Password = Helpers.Encrypt_password(entry_password.Text);

      bool status = Database.Insert(ref user);
      if (!status)
      {
        await DisplayAlert(AppResources.newaccount, AppResources.failed, AppResources.okay);
        await Cancel();
      }

      App.CurrentUser = user;

      if (Device.RuntimePlatform == Device.Android)
      {
        Application.Current.MainPage = new MainPage();
      }
      else if (Device.RuntimePlatform == Device.iOS)
      {
        await Navigation.PushModalAsync(new MainPage());
      }
    }

    /**
     * Cancel user creation
     */
    async void Btn_cancel_Clicked(object sender, EventArgs e)
    {
      await Cancel();
    }

    /**
     * Cancel methode and return to login page
     */
    async Task Cancel()
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