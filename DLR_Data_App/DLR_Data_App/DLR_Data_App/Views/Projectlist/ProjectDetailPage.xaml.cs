using System;
using DLR_Data_App.Localizations;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;

namespace DLR_Data_App.Views.ProjectList
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ProjectDetailPage
  {
    private readonly Project _workingProject;

    public ProjectDetailPage(Project project)
    {
      InitializeComponent();
      _workingProject = project;
      
      BindingContext = Helpers.TranslateProjectDetails(project);
    }

    /**
     * Default constructor only for Xamarin Form Previewer used
     */
    public ProjectDetailPage()
    {
      _workingProject = new Project
      {
        Authors = "Jan Füsting",
        Title = "Data App Example Project",
        Description = "An app to collect data for various scientific varieties."
      };

      InitializeComponent();

      BindingContext = _workingProject;
    }

    /**
     * Select project as current active project
     */
    private async void Btn_current_project_Clicked(object sender, EventArgs e)
    {
      // Set project in database as current project
      Database.SelectCurrentProject(_workingProject);

      // Navigate to current project
      if(Application.Current.MainPage is MainPage mainPage)
        await mainPage.NavigateFromMenu(0);
    }

    /**
     * Remove project from database
     */
    private async void Btn_remove_project_Clicked(object sender, EventArgs e)
    {
      var answer = await DisplayAlert(AppResources.removeproject, AppResources.removeprojectwarning, AppResources.okay, AppResources.cancel);
      if (answer)
      {
        Database.DeleteProject(_workingProject);
        await Navigation.PopAsync();
      }
    }
  }
}