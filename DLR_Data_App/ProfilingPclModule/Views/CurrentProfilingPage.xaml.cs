//Main contributors: Maximilian Enderling, Henning Woydt
using DlrDataApp.Modules.SharedModule;
using DlrDataApp.Modules.ProfilingSharedModule.Localization;
using DlrDataApp.Modules.ProfilingSharedModule.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.ProfilingSharedModule.Views
{
    public partial class CurrentProfilingPage : ContentPage
    {
        string CurrentProfilingId;
        Database Database;

        /// Constructor for the MainPage
        public CurrentProfilingPage()
        {
            InitializeComponent();
            Database = ProfilingSharedModule.Instance.ModuleHost.App.Database;
            ProfilingStorageManager.Initilize(ProfilingSharedModule.Instance.ModuleHost.App.CurrentUser.Id);

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
            var currentProfiling = Database.FindWithChildren<ActiveProfilingInfo>(t => true).ActiveProfiling;
            // Todo
            //if (currentProfiling == null)
            //{
            //    _ = App.CurrentMainPage.NavigateFromMenu(Models.MenuItemType.ProfilingList);
            //    return;
            //}

            if (CurrentProfilingId != currentProfiling.ProfilingId)
            {
                CurrentProfilingId = currentProfiling.ProfilingId;
                ProfilingStorageManager.SetProfiling(currentProfiling);
            }
            base.OnAppearing();
        }

        private void HintClicked(object sender, EventArgs e)
        {
            DisplayAlert(Localization.AppResources.hint, Localization.AppResources.hintProfilingListPage, SharedModule.Localization.AppResources.ok);
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
    }
}

