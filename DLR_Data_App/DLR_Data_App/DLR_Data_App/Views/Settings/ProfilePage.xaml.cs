using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage : ContentPage
	{
    private User selectedUser;

    /**
     * Constructor
     */
    public ProfilePage (User user)
		{
			InitializeComponent ();
      selectedUser = user;
      entry_name.Text = selectedUser.Username;
      entry_password.Text = "";
		}

    /**
     * Save changes
     */
    private async void Btn_save_Clicked(object sender, EventArgs e)
    {
      selectedUser.Username = entry_name.Text;

      if (entry_password.Text != "")
      {
        selectedUser.Password = Helpers.Encrypt_password(entry_password.Text);
      }

      bool result = Database.Update<User>(ref selectedUser);
      if (result)
      {
        await DisplayAlert(AppResources.profile, AppResources.successful, AppResources.accept);
        await Navigation.PopAsync();
      } else
      {
        // Unable to update data
        await DisplayAlert(AppResources.profile, AppResources.updateaccountfailure, AppResources.accept);
      }
    }

    /**
     * Cancel editing
     */ 
    private async void Btn_cancel_Clicked(object sender, EventArgs e)
    {
      await Navigation.PopAsync();
    }

    /**
     * Remove profile
     */
    private async void Btn_remove_Clicked(object sender, EventArgs e)
    {
      // ask user if he really wants to remove his account
      bool answer = await DisplayAlert(AppResources.removeaccount, AppResources.removeaccountwarning, AppResources.accept, AppResources.decline);

      if (answer)
      {
        // remove data from database
        bool result = Database.Delete<User>(ref selectedUser);

        // check if removal was successful
        if (result)
        {
          await DisplayAlert(AppResources.removeaccount, AppResources.successful, AppResources.accept);
          await Navigation.PopAsync();
        }
        else
        {
          await DisplayAlert(AppResources.removeaccount, AppResources.removeaccountfailure, AppResources.accept);
        }
      }
    }
  }
}