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
        public ObservableCollection<SurveyMenuItem> Items = new ObservableCollection<SurveyMenuItem>()
        {
            new SurveyMenuItem(SurveyMenuItemType.DoubleSlider, "Bedeckungsgrade", 4, 18, 0, true, "{AnswersGiven}/{AnswersNeeded} ({MaximumQuestionNumber})", Color.White), //Hintergrundfarbe hab ich Weiß gemacht
            new SurveyMenuItem(SurveyMenuItemType.ImageChecker, "Sortenerkennung", 8, 25, 0, true, "{AnswersGiven}/{AnswersNeeded} ({MaximumQuestionNumber})", Color.White),
            new SurveyMenuItem(SurveyMenuItemType.Introspection, "Selbsteinschätzung", 5, 16, 0, true, "{AnswersGiven}/{AnswersNeeded} ({MaximumQuestionNumber})", Color.White),
            new SurveyMenuItem(SurveyMenuItemType.Stadium, "Wuchsstadien", 2, 12, 0, true, "{AnswersGiven}/{AnswersNeeded} ({MaximumQuestionNumber})", Color.White)
        };
        Dictionary<SurveyMenuItemType, Func<ContentPage>> PageConstructorDictionary = new Dictionary<SurveyMenuItemType, Func<ContentPage>>()
        {
            { SurveyMenuItemType.DoubleSlider, () => new DoubleSliderPage() },
            { SurveyMenuItemType.Stadium, () => new StadiumPage() },
            { SurveyMenuItemType.ImageChecker, () => new ImageCheckerPage() },
            { SurveyMenuItemType.Introspection, () => new IntrospectionPage() }
        };
        
        public MainPage()
        {
            InitializeComponent();
            MenuList.ItemsSource = Items;
        }

        private async void MenuList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is SurveyMenuItem selectedItem))
                throw new NotImplementedException();
            SurveyMenuItem tapped = (SurveyMenuItem)e.Item;
            if (PageConstructorDictionary.TryGetValue(selectedItem.Id, out var pageConstructor))
            {
                await Navigation.PushAsync(pageConstructor());
            }
                
        }
        private async void EvaluationClicked(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new EvaluationMainPage());     
        }

    }
}

