//Main contributors: Maximilian Enderling, Henning Woydt
using DLR_Data_App.Localizations;
using DLR_Data_App.Models.Profiling;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Views.Profiling
{
    public partial class CurrentProfilingPage : ContentPage
    {
        string CurrentProfilingId;

        /// Constructor for the MainPage
        public CurrentProfilingPage()
        {
            InitializeComponent();

            ProfilingStorageManager.Initilize(App.CurrentUser.Id);

            MenuList.ItemsSource = ProfilingStorageManager.ProfilingMenuItems;
        }

        /// Defining the Event for the click on an element in the ListView
        private void MenuList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is ProfilingMenuItem selectedItem))
                throw new NotImplementedException();
            ProfilingMenuItem tapped = (ProfilingMenuItem)e.Item;
            //Navigate to the clicked element in the list
            ProfilingManager.StartProfiling(tapped);
        }

        protected override void OnAppearing()
        {
            var currentProfiling = Database.GetCurrentProfiling();
            if (currentProfiling == null)
            {
                _ = App.CurrentMainPage.NavigateFromMenu(Models.MenuItemType.ProfilingList);
                return;
            }

            if (CurrentProfilingId != currentProfiling.ProfilingId)
            {
                CurrentProfilingId = currentProfiling.ProfilingId;
                ProfilingStorageManager.SetProfiling(currentProfiling);
            }
            base.OnAppearing();
        }

        private void HintClicked(object sender, EventArgs e)
        {
            DisplayAlert(AppResources.hint, AppResources.hintProfilingListPage, AppResources.ok);
        }

        private async void EvaluationClicked(object sender, EventArgs e)
        {
            var evalItems = ProfilingStorageManager.ProfilingMenuItems.Select(i => i.IntrospectionQuestion.All(q => ProfilingStorageManager.DoesAnswersExists("Introspection", q)) ? 
                ProfilingManager.GenerateEvaluationItem(i) :
                new EvaluationItem(i.ChapterName, -1, -1, -1, -1)).ToList();
            
            await this.PushPage(new EvaluationMainPage(evalItems));
        }

        private void ExportAnwersClicked(object sender, EventArgs e)
        {
            ProfilingStorageManager.ExportAnswers();
        }

        private void DeleteAnswersClicked(object sender, EventArgs e)
        {
            ProfilingStorageManager.ResetSavedAnswers();
        }
    }
}

