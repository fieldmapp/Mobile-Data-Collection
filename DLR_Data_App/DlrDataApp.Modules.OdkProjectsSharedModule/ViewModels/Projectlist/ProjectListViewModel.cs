using DlrDataApp.Modules.OdkProjectsSharedModule.Localization;
using DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectModel;
using DlrDataApp.Modules.SharedModule;
using System.Collections.ObjectModel;
namespace DlrDataApp.Modules.OdkProjectsSharedModule.ViewModels.ProjectList
{
    public class ProjectListViewModel : BaseViewModel
    {
        public ObservableCollection<Project> Projects { get; set; }

        public ProjectListViewModel()
        {
            Title = OdkProjectsResources.projects;
            Projects = new ObservableCollection<Project>();
        }

        /// <summary>
        /// Updates the project list.
        /// </summary>
        public void UpdateProjects()
        {
            //var projectList = Database.ReadProjects();
            var projectListTranslated = Services.Helpers.TranslateProjectDetails(Database.ReadProjects());

            if (Projects == null) return;

            Projects.Clear();
            foreach (var project in projectListTranslated)
            {
                Projects.Add(project);
            }
        }
    }
}