using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.ViewModels;
using DLR_Data_App.Services;
using System.Collections.Generic;

namespace DLR_Data_App.Views.Projectlist
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ItemDetailPage : ContentPage
  {
    private Project workingProject;

    public ItemDetailPage(Project project)
    {
      InitializeComponent();
      workingProject = project;
      
      BindingContext = Helpers.TranslateProjectDetails(project);
    }

    /**
     * Default constructor only for Xamarin Form Previewer used
     */
    public ItemDetailPage()
    {
      workingProject = new Project();
      workingProject.Authors = "Jan Füsting";
      workingProject.Title = "Data App Example Project";
      workingProject.Description = "An app to collect data for various scientific varieties.";

      InitializeComponent();

      BindingContext = workingProject;
    }

    /**
     * Select project as current active project
     */
    private void Btn_current_project_Clicked(object sender, EventArgs e)
    {
      // Entry point for Profiling, currently directed to project
      Database.SelectCurrentProject(workingProject);
    }

    /**
     * Remove project from database
     */
    private void Btn_remove_project_Clicked(object sender, EventArgs e)
    {

    }
  }
}