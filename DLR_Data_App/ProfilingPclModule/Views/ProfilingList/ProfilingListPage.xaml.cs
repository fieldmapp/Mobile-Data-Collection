using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using DlrDataApp.Modules.ProfilingSharedModule.Models;
using DlrDataApp.Modules.SharedModule;

namespace DlrDataApp.Modules.ProfilingSharedModule.Views.ProfilingList
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilingListPage
    {
        public class LocalizedProfilingDataWrapper
        {
            public string LocalizedTitle { get; set; }

            public string LocalizedDescription { get; set; }

            public string LocalizedAuthors { get; set; }

            public ProfilingData ProfilingData { get; set; }
        }
        public ObservableCollection<LocalizedProfilingDataWrapper> Profilings { get; set; }
        private NewProfilingPage _newProfilingPage;
        
        public ProfilingListPage()
        {
            InitializeComponent();

            Profilings = new ObservableCollection<LocalizedProfilingDataWrapper>();
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
            foreach (var profiling in ProfilingSharedModule.Instance.ModuleHost.App.Database.Read<ProfilingData>())
            {
                Profilings.Add(new LocalizedProfilingDataWrapper
                {
                    LocalizedAuthors = Helpers.GetCurrentLanguageTranslation(profiling.Translations, profiling.Authors),
                    LocalizedDescription = Helpers.GetCurrentLanguageTranslation(profiling.Translations, profiling.Description),
                    LocalizedTitle = Helpers.GetCurrentLanguageTranslation(profiling.Translations, profiling.Title),
                    ProfilingData = profiling
                });
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
            await this.PushPage(new ProfilingDetailPage(Profilings[e.ItemIndex].ProfilingData));
        }
    }
}