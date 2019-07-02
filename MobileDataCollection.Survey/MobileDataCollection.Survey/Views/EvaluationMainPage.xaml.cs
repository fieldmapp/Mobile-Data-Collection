using MobileDataCollection.Survey.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDataCollection.Survey.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EvaluationMainPage : ContentPage
    {
        public ObservableCollection<EvaluationItem> EvaluationItems = new ObservableCollection<EvaluationItem>()
        {
            new EvaluationItem("Bedeckungsgrade", 90, 100, 89, 81),
            new EvaluationItem("Sortenerkennung", 35, 50, 30,21),
            new EvaluationItem("Wuchsstadien", 70, 80,51,23) 
        };
        public EvaluationMainPage()
        {
            InitializeComponent();
            CatList.ItemsSource = EvaluationItems;
        }
        
        private async void DetailsClicked(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is EvaluationItem selectedItem))
                throw new NotImplementedException();
            EvaluationItem tapped = (EvaluationItem)e.Item;
            await Navigation.PushAsync(new EvaluationDetailsPage(tapped.PercentEasy, tapped.PercentMedium, tapped.PercentHard));
        }
    }
}