using MobileDataCollection.Survey.Controls;
using MobileDataCollection.Survey.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDataCollection.Survey.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IntrospectionPage : ContentPage
	{
        public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof(Item), 
            typeof(QuestionIntrospectionPage), typeof(IntrospectionPage), new QuestionIntrospectionPage("demo"), 
            BindingMode.OneWay);

        public QuestionIntrospectionPage Item
        {
            get { return (QuestionIntrospectionPage)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public ObservableCollection<QuestionIntrospectionPage> Items
        { get; set; }
        public IntrospectionPage()
        {
            InitializeComponent();
            QuestionLabel.BindingContext = this;
            Item = new QuestionIntrospectionPage("Ich kann eine Sorte von Feldfrüchten zuverlässig erkennen.");
        }
            public IntrospectionPage(SurveyMenuItem survey)
        {
            InitializeComponent();
            QuestionLabel.BindingContext = this;
            if (survey.Id == SurveyMenuItemType.ImageChecker) Items = new ObservableCollection<QuestionIntrospectionPage>() {
                new QuestionIntrospectionPage("Ich kann eine Sorte von Feldfrüchten zuverlässig erkennen.")}; 
            else if (survey.Id == SurveyMenuItemType.DoubleSlider) {
                Items = new ObservableCollection<QuestionIntrospectionPage>(){
                    new QuestionIntrospectionPage("Ich kann den Bedeckungsgrad des Bodens durch Pflanzen zuverlässig schätzen."),
                    new QuestionIntrospectionPage("Ich kann den Anteil grüner Pflanzenbestandteile am gesamten Pflanzenmaterial zuverlässig schätzen.")
                };
            }
            else if (survey.Id == SurveyMenuItemType.Stadium){ Items = new ObservableCollection<QuestionIntrospectionPage>() {
                new QuestionIntrospectionPage("Ich kann phänologische Entwicklungsstadien von Feldfrüchten zuverlässig erkennen.")};
            }
            this.Item = Items[0];
        }

        private void Button_Tapped(object sender, EventArgs e)
        {
            Button1.IsChecked = false;
            Button2.IsChecked = false;
            Button3.IsChecked = false;
            Button4.IsChecked = false;
            Button5.IsChecked = false;

            (sender as RadioButton).IsChecked = true;
        }
    }
}