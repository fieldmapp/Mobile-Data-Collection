//Main contributors: Maximilian Enderling, Henning Woydt
using DLR_Data_App.Models.Survey;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Views.Survey
{
    public partial class SurveyListPage : ContentPage
    {
        /// Constructor for the MainPage
        public SurveyListPage()
        {
            InitializeComponent();

            SurveyManager.Initialize(App.CurrentUser.Id.ToString());
            
            MenuList.ItemsSource = DatabankCommunication.SurveyMenuItems;
        }
        /// Defining the Event for the click on an element in the ListView
        private void MenuList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is SurveyMenuItem selectedItem))
                throw new NotImplementedException();
            SurveyMenuItem tapped = (SurveyMenuItem)e.Item;
            ///Navigate to the clicked element in the list
            SurveyManager.StartSurvey(tapped);
        }

        private void HintClicked(object sender, EventArgs e)
        {
            DisplayAlert("Hinweis", "Hier sehen Sie alle verfügbaren Kategorien des Quiz. \n" +
                "Klicken Sie auf eine Kategorie, um Fragen in dieser zu beantworten. Sie sehen für jede Kategorie:\n" +
                "- wieviele Fragen Sie bereits beantwortet haben (links) \n" +
                "- wieviele Fragen Sie mindestens beantworten müssen, um die Kategorie abzuschließen (rechts)", "OK");
        }

        private async void EvaluationClicked(object sender, EventArgs e)
        {
            var evalItems = DatabankCommunication.SurveyMenuItems.Select(i => i.IntrospectionQuestion.All(q => DatabankCommunication.DoesAnswersExists("Introspection", q)) ? 
                SurveyManager.GenerateEvaluationItem(i) :
                new EvaluationItem(i.ChapterName, -1, -1, -1, -1)).ToList();
            
            await this.PushPage(new EvaluationMainPage(evalItems));
        }

        private void ExportAnwersClicked(object sender, EventArgs e)
        {
            DatabankCommunication.ExportAnswers();
        }

        private void DeleteAnswersClicked(object sender, EventArgs e)
        {
            DatabankCommunication.ResetSavedAnswers();
        }
    }
}

