using MobileDataCollection.Survey.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Views
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// This is the csharp representation of the main page. It has the property Items, which is an collection of all 
        /// question categories and a SurveyManager handling the navigation
        /// </summary>
        public ObservableCollection<SurveyMenuItem> Items = new ObservableCollection<SurveyMenuItem>()
        {
            new SurveyMenuItem(SurveyMenuItemType.DoubleSlider, "Bedeckungsgrade", 4, 18, 0, true, Color.White, new List<int>{3,4}),
            new SurveyMenuItem(SurveyMenuItemType.ImageChecker, "Sortenerkennung", 8, 25, 0, true, Color.White, new List<int>{2}),
            new SurveyMenuItem(SurveyMenuItemType.Stadium, "Wuchsstadien", 2, 12, 0, true, Color.White, new List<int>{1})
        };
        SurveyManager SurveyManager;
        /// Constructor for the MainPage
        public MainPage()
        {
            ///Initialize components
            InitializeComponent();
            ///Settings the ItemsSource for the ListView containing the question categories
            MenuList.ItemsSource = Items;
            ///Setting the SurveyManager
            SurveyManager = new SurveyManager(Navigation);
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

        private async void EvaluationClicked(object sender, ItemTappedEventArgs e)
        {
            var evalItems = Items.Select(i => i.IntrospectionQuestion.All(q => DatabankCommunication.SearchAnswers("Introspection", q)) ? 
            SurveyManager.GenerateEvaluationItem(i) :
            new EvaluationItem(i.ChapterName, -1, -1, -1, -1)).ToList();
            await Navigation.PushAsync(new EvaluationMainPage(evalItems));
        }

    }
}

