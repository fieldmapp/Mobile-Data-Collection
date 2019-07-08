using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using DLR_Data_App.Models;
using DLR_Data_App.Views;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;

namespace DLR_Data_App.ViewModels.Projectlist
{
  public class ItemsViewModel : BaseViewModel
  {
    public ObservableCollection<Project> Projects { get; set; }

    public ItemsViewModel()
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

      if(Projects != null)
      {
        Projects.Clear();
        foreach (Project project in projectListTranslated)
        {
          Projects.Add(project);
        }
      }
    }
  }
}