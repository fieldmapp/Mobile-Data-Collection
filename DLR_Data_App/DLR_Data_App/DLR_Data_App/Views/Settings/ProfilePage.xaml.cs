using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System;

using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
  /**
   * This class handles the profile information
   */
  [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage
	{
    private User _selectedUser;

    /**
     * Constructor
     * @param user Chosen user from list
     */
    public ProfilePage (User user)
		{
			InitializeComponent ();
      _selectedUser = user;
      EntryName.Text = _selectedUser.Username;
      EntryPassword.Text = "";
		}

    /**
     * Save changes
     */
    private async void Btn_save_Clicked(object sender, EventArgs e)
    {
      _selectedUser.Username = EntryName.Text;

      if (EntryPassword.Text != "")
      {
        _selectedUser.Password = Helpers.Encrypt_password(EntryPassword.Text);
      }

      var result = Database.Update(ref _selectedUser);
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
      var answer = await DisplayAlert(AppResources.removeaccount, AppResources.removeaccountwarning, AppResources.accept, AppResources.decline);

      if (!answer) return;

      // remove data from database
      var result = Database.Delete(ref _selectedUser);

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