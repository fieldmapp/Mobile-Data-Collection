using System.Collections.ObjectModel;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;

namespace DLR_Data_App.ViewModels.ProjectList
{
    public class ProjectListViewModel : BaseViewModel
    {
        public ObservableCollection<Project> Projects { get; set; }

        public ProjectListViewModel()
        {
            Title = AppResources.projects;
            Projects = new ObservableCollection<Project>();
        }

        /// <summary>
        /// Updates the project list.
        /// </summary>
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