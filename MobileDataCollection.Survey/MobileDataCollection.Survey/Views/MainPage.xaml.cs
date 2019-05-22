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
            new SurveyMenuItem(){AnswersGiven=2, AnswersNeeded=2, ChapterName="DoubleSlider", Id= SurveyMenuItemType.DoubleSlider, Unlocked=true},
            new SurveyMenuItem(){AnswersGiven=3, AnswersNeeded=2, ChapterName="ImageChecker", Id= SurveyMenuItemType.ImageChecker, Unlocked=true},
            new SurveyMenuItem(){AnswersGiven=1, AnswersNeeded=2, ChapterName="Introspection", Id= SurveyMenuItemType.Introspection, Unlocked=true},
            new SurveyMenuItem(){AnswersGiven=0, AnswersNeeded=2, ChapterName="Stadium", Id= SurveyMenuItemType.Stadium, Unlocked=false}
        };

        public MainPage()
        {
            InitializeComponent();
            MenuList.ItemsSource = Items;
        }
    }
}
