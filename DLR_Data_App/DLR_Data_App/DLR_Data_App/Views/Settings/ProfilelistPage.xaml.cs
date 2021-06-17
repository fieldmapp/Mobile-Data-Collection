using DLR_Data_App.Models;
using DLR_Data_App.Services;
using DlrDataApp.Modules.Base.Shared;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileListPage
    {
        private readonly List<User> _userList;

        public ProfileListPage()
        {
            InitializeComponent();
            // Get all users from database
            _userList = (App.Current as App).Database.Read<User>();
            ProfileListView.ItemsSource = _userList;
        }

        /// <summary>
        /// Show details of selected user
        /// </summary>
        private async void ProfileListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            User selectedUser = _userList[e.ItemIndex];
            await this.PushPage(new ProfilePage(selectedUser));
        }

    }
}