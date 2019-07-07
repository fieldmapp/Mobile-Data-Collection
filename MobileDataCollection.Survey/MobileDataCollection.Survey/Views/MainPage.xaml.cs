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
            new SurveyMenuItem(SurveyMenuItemType.DoubleSlider, "Bedeckungsgrade", 4, 18, 0, true, Color.White),
            new SurveyMenuItem(SurveyMenuItemType.ImageChecker, "Sortenerkennung", 8, 25, 0, true, Color.White),
            new SurveyMenuItem(SurveyMenuItemType.Stadium, "Wuchsstadien", 2, 12, 0, true, Color.White)
        };
        SurveyManager SurveyManager;

        public MainPage()
        {
            InitializeComponent();
            MenuList.ItemsSource = Items;
            SurveyManager = new SurveyManager(Navigation);
        }

        private void MenuList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is SurveyMenuItem selectedItem))
                throw new NotImplementedException();
            SurveyMenuItem tapped = (SurveyMenuItem)e.Item;
            SurveyManager.StartSurvey(tapped);
        }

        private async void EvaluationClicked(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new EvaluationMainPage());
        }

    }
}

