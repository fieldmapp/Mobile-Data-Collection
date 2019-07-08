using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.ViewModels
{
  /**
   * Class for presenting current project
   */
  class ProjectViewModel : BaseViewModel
  {
    public Project project;
    public Project translatedProject;

    public ProjectViewModel()
    {
      Title = AppResources.currentproject;
    }

    /**
     * Update view
     */
    public void UpdateView()
    {
      // Get current project
      project = Database.GetCurrentProject();
      
      // Check if current project is set
      if (project == null)
      {
        Title = AppResources.currentproject;
      }
      else
      {
        translatedProject = Helpers.TranslateProjectDetails(project);
        Title = translatedProject.Title;
      }
    }
  }
}
