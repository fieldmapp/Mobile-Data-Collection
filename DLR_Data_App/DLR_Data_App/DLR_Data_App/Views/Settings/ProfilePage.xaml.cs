using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System;

using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage
    {
        private User _selectedUser;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="user">User Chosen user from list</param>
        public ProfilePage(User user)
        {
            InitializeComponent();
            _selectedUser = user;
            EntryName.Text = _selectedUser.Username;
        }

        /// <summary>
        /// Cancel editing
        /// </summary>
        private async void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Remove profile
        /// </summary>
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