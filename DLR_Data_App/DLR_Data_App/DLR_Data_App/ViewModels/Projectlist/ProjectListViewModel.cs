using System.Collections.ObjectModel;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;

namespace DLR_Data_App.ViewModels.ProjectList
{
  /**
   * View model for all projects
   */
  public class ProjectListViewModel : BaseViewModel
  {
    public ObservableCollection<Project> Projects { get; set; }

    /**
     * Constructor
     */
    public ProjectListViewModel()
    {
      Title = AppResources.projects;
      Projects = new ObservableCollection<Project>();
    }

    /**
     * Update project list
     */
    public void UpdateProjects()
    {
      //var projectList = Database.ReadProjects();
      var projectListTranslated = Helpers.TranslateProjectDetails(Database.ReadProjects());

      if (Projects == null) return;

      Projects.Clear();
      foreach (var project in projectListTranslated)
      {
        Projects.Add(project);
      }
    }
  }
}