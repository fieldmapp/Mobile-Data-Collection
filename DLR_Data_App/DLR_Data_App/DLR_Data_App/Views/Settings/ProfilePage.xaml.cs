using DLR_Data_App.Models;
using DLR_Data_App.Views.Login;
using DlrDataApp.Modules.Base.Shared.Localization;
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
            var answer = await DisplayAlert(SharedResources.removeaccount, SharedResources.removeaccountwarning, SharedResources.accept, SharedResources.decline);

            if (!answer) return;

            var isActiveUser = App.Current.CurrentUser.Id == _selectedUser.Id;

            // remove data from database
            var result = App.Current.Database.Delete(_selectedUser);

            // check if removal was successful
            if (result)
            {
                await DisplayAlert(SharedResources.removeaccount, SharedResources.successful, SharedResources.accept);
                if (isActiveUser)
                    _ = AppShell.Current.GoToAsync("//logout");
                else
                    _ = Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert(SharedResources.removeaccount, SharedResources.removeaccountfailure, SharedResources.accept);
            }
        }
    }
}