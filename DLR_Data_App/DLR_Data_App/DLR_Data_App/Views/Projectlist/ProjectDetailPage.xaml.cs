using System;
using DLR_Data_App.Localizations;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using DLR_Data_App.Models;

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

        /// <summary>
        /// Default constructor only to be used by the Xamarin Form Previewer 
        /// </summary>
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

        /// <summary>
        /// Selects project as current active project.
        /// </summary>
        private async void Btn_current_project_Clicked(object sender, EventArgs e)
        {
            // Set project in database as current project
            Database.SetCurrentProject(_workingProject);

            // Navigate to current project
            await App.CurrentMainPage.NavigateFromMenu(MenuItemType.CurrentProject);
        }

        /// <summary>
        /// Removes project from database.
        /// </summary>
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