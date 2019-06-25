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
            new SurveyMenuItem(){AnswersGiven=0, AnswersNeeded=4, MaximumQuestionNumber=18, ChapterName="Bedeckungsgrade", Id= SurveyMenuItemType.DoubleSlider, Unlocked=true},
            new SurveyMenuItem(){AnswersGiven=0, AnswersNeeded=8, MaximumQuestionNumber=25, ChapterName="Sortenerkennung", Id= SurveyMenuItemType.ImageChecker, Unlocked=true},
            new SurveyMenuItem(){AnswersGiven=0, AnswersNeeded=5, MaximumQuestionNumber=16, ChapterName="Selbsteinschätzung", Id= SurveyMenuItemType.Introspection, Unlocked=true},
            new SurveyMenuItem(){AnswersGiven=0, AnswersNeeded=2, MaximumQuestionNumber=12, ChapterName="Wuchsstadien", Id= SurveyMenuItemType.Stadium, Unlocked=true}
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
    }
}
