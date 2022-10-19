using DlrDataApp.Modules.OdkProjects.Shared.Localization;
using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel;
using DlrDataApp.Modules.Base.Shared;
using System.Collections.ObjectModel;

namespace DlrDataApp.Modules.OdkProjects.Shared.ViewModels.ProjectList
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
            var projectListTranslated = Services.Helpers.TranslateProjectDetails(OdkProjectsModule.Instance.Database.ReadWithChildren<Project>());

            if (Projects == null) return;

            Projects.Clear();
            foreach (var project in projectListTranslated)
            {
                Projects.Add(project);
            }
        }
    }
}