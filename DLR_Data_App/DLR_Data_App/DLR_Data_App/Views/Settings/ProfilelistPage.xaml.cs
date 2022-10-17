using DLR_Data_App.Models;
using DLR_Data_App.Services;
using DlrDataApp.Modules.Base.Shared;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileListPage
    {
        private readonly ObservableCollection<User> _userList;

        public ProfileListPage()
        {
            InitializeComponent();
            // Get all users from database
            _userList = new ObservableCollection<User>((App.Current as App).Database.Read<User>());
            ProfileListView.ItemsSource = _userList;
        }

        protected override void OnAppearing()
        {
            _userList.SetTo(App.Current.Database.Read<User>());
        }

        /// <summary>
        /// Show details of selected user
        /// </summary>
        private async void ProfileListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            User selectedUser = _userList[e.ItemIndex];
            await Shell.Current.Navigation.PushPage(new ProfilePage(selectedUser));
        }

    }
}