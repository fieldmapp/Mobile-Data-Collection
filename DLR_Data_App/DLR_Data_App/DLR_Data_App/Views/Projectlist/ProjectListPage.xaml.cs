using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.ViewModels.ProjectList;
using DLR_Data_App.Services;
using DLR_Data_App.Localizations;

namespace DLR_Data_App.Views.ProjectList
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectListPage
    {
        private List<Project> _projectList;
        private NewProjectPage _newProjectPage;

        private readonly ProjectListViewModel _viewModel;
        
        public ProjectListPage()
        {
            InitializeComponent();

            _viewModel = new ProjectListViewModel();
            BindingContext = _viewModel;
        }

        /// <summary>
        /// Opens a <see cref="NewProjectPage"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            Application.Current.ModalPopping += HandleModalPopping;
            _newProjectPage = new NewProjectPage();
            await Navigation.PushModalAsync(_newProjectPage);
        }

        /// <summary>
        /// Handles refreshing the list after adding a new project.
        /// <see cref="https://stackoverflow.com/questions/39652909/await-for-a-pushmodalasync-form-to-closed-in-xamarin-forms"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleModalPopping(object sender, ModalPoppingEventArgs e)
        {
            _projectList = Database.ReadProjects();
            _viewModel.UpdateProjects();

            // remember to remove the event handler:
            Application.Current.ModalPopping -= HandleModalPopping;
        }

        /// <summary>
        /// Refreshs list
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _projectList = Database.ReadProjects();

            _viewModel.UpdateProjects();
        }

        /// <summary>
        /// Opens detail of project
        /// </summary>
        private async void ProjectListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await this.PushPage(new ProjectDetailPage(_projectList[e.ItemIndex]));
        }
    }
}