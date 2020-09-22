using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.ViewModels.ProjectList;
using DLR_Data_App.Services;
using DLR_Data_App.Localizations;
using DLR_Data_App.Models.Profiling;
using System.Collections.ObjectModel;

namespace DLR_Data_App.Views.ProfilingList
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilingListPage
    {
        public ObservableCollection<ProfilingData> Profilings { get; set; }
        private NewProfilingPage _newProfilingPage;
        
        public ProfilingListPage()
        {
            InitializeComponent();

            Profilings = new ObservableCollection<ProfilingData>();
            BindingContext = this;
        }

        /// <summary>
        /// Opens a <see cref="NewProjectPage"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            Application.Current.ModalPopping += HandleModalPopping;
            _newProfilingPage = new NewProfilingPage();
            await Navigation.PushModalAsync(_newProfilingPage);
        }

        /// <summary>
        /// Handles refreshing the list after adding a new project.
        /// <see cref="https://stackoverflow.com/questions/39652909/await-for-a-pushmodalasync-form-to-closed-in-xamarin-forms"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleModalPopping(object sender, ModalPoppingEventArgs e)
        {
            Application.Current.ModalPopping -= HandleModalPopping;

            RefreshProfilingList();
        }

        private void RefreshProfilingList()
        {
            Profilings.Clear();
            foreach (var profiling in Database.ReadProfilings())
            {
                Profilings.Add(profiling);
            }
        }

        /// <summary>
        /// Refreshs list
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshProfilingList();
        }

        /// <summary>
        /// Opens detail of project
        /// </summary>
        private async void ProjectListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await this.PushPage(new ProfilingDetailPage(Profilings[e.ItemIndex]));
        }
    }
}