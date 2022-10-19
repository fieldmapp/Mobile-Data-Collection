using DlrDataApp.Modules.Base.Shared.Localization;
using DlrDataApp.Modules.OdkProjects.Shared.Localization;
using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjects.Shared.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.OdkProjects.Shared.Views.ProjectList
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

        /// <summary>
        /// Default constructor only to be used by the Xamarin Form Previewer 
        /// </summary>
        public ProjectDetailPage()
        {
            _workingProject = new Project
            {
                Authors = "Author",
                Title = "Project title",
                Description = "Project description"
            };

            InitializeComponent();

            BindingContext = _workingProject;
        }

        /// <summary>
        /// Selects project as current active project.
        /// </summary>
        private void Btn_current_project_Clicked(object sender, EventArgs e)
        {
            // Set project in database as current project
            OdkProjectsModule.Instance.Database.SetActiveElement<Project, ActiveProjectInfo>(_workingProject);

            // Navigate to current project
            _ = Shell.Current.GoToAsync("//projectscurrent");
        }

        /// <summary>
        /// Removes project from database.
        /// </summary>
        private async void Btn_remove_project_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(OdkProjectsResources.removeproject, OdkProjectsResources.removeprojectwarning, SharedResources.okay, SharedResources.cancel);
            if (answer)
            {
                OdkProjectsModule.Instance.Database.Delete(_workingProject);
                await Navigation.PopAsync();
            }
        }
    }
}