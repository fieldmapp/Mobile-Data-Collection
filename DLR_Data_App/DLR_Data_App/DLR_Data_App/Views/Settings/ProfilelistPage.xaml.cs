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
  /**
   * Lists all existing profiles from database
   */
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ProfilelistPage : ContentPage
  {
    private List<User> userList;

    public ProfilelistPage()
    {
      InitializeComponent();
      // Get all users from database
      userList = Database.ReadUser();
    }

    /**
     * Refresh list on each new appearing
     */
    protected override void OnAppearing()
    {
      base.OnAppearing();
      
      profileListView.ItemsSource = userList;
    }

    /**
     * Show details of selected user 
     */
    private async void ProfileListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
      User selectedUser = userList[e.ItemIndex];
      await Navigation.PushAsync(new ProfilePage(selectedUser));
    }
    
  }
}