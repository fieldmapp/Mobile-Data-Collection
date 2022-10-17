//Main contributors: Maximilian Enderling, Henning Woydt
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Profiling.Shared.Localization;
using DlrDataApp.Modules.Profiling.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using DlrDataApp.Modules.Base.Shared.Localization;

namespace DlrDataApp.Modules.Profiling.Shared.Views
{
    public partial class CurrentProfilingPage : ContentPage
    {
        string CurrentProfilingId;
        Database Database;

        /// Constructor for the MainPage
        public CurrentProfilingPage()
        {
            InitializeComponent();
            Database = ProfilingModule.Instance.ModuleHost.App.Database;
            ProfilingStorageManager.Initilize(ProfilingModule.Instance.ModuleHost.App.CurrentUser.Id.Value);

            MenuList.ItemsSource = ProfilingStorageManager.ProfilingMenuItems;
            Shell.Current.Navigated += Current_Navigated;
        }

        private void Current_Navigated(object sender, ShellNavigatedEventArgs e)
        {
            if (e.Current.Location.OriginalString != "//profilingcurrent")
                return;

            var currentProfiling = Database.FindWithChildren<ActiveProfilingInfo>(t => true, true)?.ActiveProfiling;

            if (CurrentProfilingId != currentProfiling.ProfilingId)
            {
                CurrentProfilingId = currentProfiling.ProfilingId;
                ProfilingStorageManager.SetProfiling(currentProfiling);
            }
        }

        /// Defining the Event for the click on an element in the ListView
        private void MenuList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is ProfilingMenuItem selectedItem))
                throw new NotImplementedException();
            //Navigate to the clicked element in the list
            ProfilingManager.StartProfiling(selectedItem);
        }

        private void HintClicked(object sender, EventArgs e)
        {
            DisplayAlert(ProfilingResources.hint, ProfilingResources.hintProfilingListPage, SharedResources.ok);
        }

        private async void EvaluationClicked(object sender, EventArgs e)
        {
            var evalItems = ProfilingStorageManager.ProfilingMenuItems.Select(i => i.IntrospectionQuestion.All(q => ProfilingStorageManager.DoesAnswersExists("Introspection", q)) ? 
                ProfilingManager.GenerateEvaluationItem(i) :
                new EvaluationItem(i.ChapterName, -1, -1, -1, -1)).ToList();
            
            await Shell.Current.Navigation.PushPage(new EvaluationMainPage(evalItems));
        }

        private async void ExportAnwersClicked(object sender, EventArgs e)
        {
            ProfilingStorageManager.ExportAnswers();
            await DisplayAlert(SharedResources.save, SharedResources.answersExportedToRoot, SharedResources.okay);
        }
    }
}

