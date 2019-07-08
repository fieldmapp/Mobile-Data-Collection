using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Views;
using DLR_Data_App.ViewModels.Projectlist;
using DLR_Data_App.Services;
using System.Collections.ObjectModel;

namespace DLR_Data_App.Views.Projectlist
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ItemsPage : ContentPage
  {
    List<Project> projectList;

    ItemsViewModel viewModel;

    /**
     * Constructor
     */
    public ItemsPage()
    {
      InitializeComponent();

      viewModel = new ItemsViewModel();
      BindingContext = viewModel;
    }

    /**
     * Open new project dialog
     */
    async void AddItem_Clicked(object sender, EventArgs e)
    {
      await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
    }

    /**
     * Refresh list
     */
    protected override void OnAppearing()
    {
      base.OnAppearing();
      projectList = Database.ReadProjects();

      viewModel.UpdateProjects();
    }

    /**
     * Open details of project
     */
    private async void ProjectListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
      await Navigation.PushAsync(new ItemDetailPage(projectList[e.ItemIndex]));
    }
  }
}