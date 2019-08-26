using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
  /**
   * Lists all existing profiles from database
   */
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ProfileListPage
  {
    private readonly List<User> _userList;

    public ProfileListPage()
    {
      InitializeComponent();
      // Get all users from database
      _userList = Database.ReadUser();
    }

    /**
     * Refresh list on each new appearing
     */
    protected override void OnAppearing()
    {
      base.OnAppearing();
      
      ProfileListView.ItemsSource = _userList;
    }

    /**
     * Show details of selected user 
     */
    private async void ProfileListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
      User selectedUser = _userList[e.ItemIndex];
      await Navigation.PushAsync(new ProfilePage(selectedUser));
    }
    
  }
}