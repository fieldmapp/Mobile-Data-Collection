﻿using MobileDataCollection.Survey.Controls;
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
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem), typeof(QuestionIntrospectionPage), typeof(IntrospectionPage), new QuestionIntrospectionPage("demo"), BindingMode.OneWay);
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem), typeof(AnswerIntrospectionPage), typeof(IntrospectionPage), new AnswerIntrospectionPage(new QuestionIntrospectionPage("demo"), 1), BindingMode.OneWay);

        public QuestionIntrospectionPage QuestionItem
        {
            get { return (QuestionIntrospectionPage)GetValue(QuestionItemProperty); }
            set { SetValue(QuestionItemProperty, value); }
        }

        public AnswerIntrospectionPage AnswerItem
        {
            get { return (AnswerIntrospectionPage)GetValue(AnswerItemProperty); }
            set { SetValue(AnswerItemProperty, value); }
        }

        public ObservableCollection<QuestionIntrospectionPage> Items
        { get; set; }
        public IntrospectionPage()
        {
            InitializeComponent();
            QuestionLabel.BindingContext = this;
            QuestionItem = new QuestionIntrospectionPage("Ich kann eine Sorte von Feldfrüchten zuverlässig erkennen.");
            RadioButtonIndex = new Dictionary<RadioButton, int>()
            {
                {Button1, 1},
                {Button2, 2},
                {Button3, 3},
                {Button4, 4},
                {Button5, 5}
            };
        }
            public IntrospectionPage(SurveyMenuItem survey)
        {
            InitializeComponent();
            QuestionLabel.BindingContext = this;
            //Ans Ende jeder Survey müssen Introspectionfrage(n), d.h. für jeden Survey-Typ müssen 1-n Introspection fragen hinterlegbar 
            if (survey.Id == SurveyMenuItemType.ImageChecker) Items = new ObservableCollection<QuestionIntrospectionPage>() {
                new QuestionIntrospectionPage("Ich kann eine Sorte von Feldfrüchten zuverlässig erkennen.")}; 
            else if (survey.Id == SurveyMenuItemType.DoubleSlider) {
                Items = new ObservableCollection<QuestionIntrospectionPage>(){
                    //Für DoubleSliderPage werden zwei Introspection-Fragen gebraucht
                    new QuestionIntrospectionPage("Ich kann den Bedeckungsgrad des Bodens durch Pflanzen zuverlässig schätzen."),
                    new QuestionIntrospectionPage("Ich kann den Anteil grüner Pflanzenbestandteile am gesamten Pflanzenmaterial zuverlässig schätzen.")
                };
            }
            else if (survey.Id == SurveyMenuItemType.Stadium){ Items = new ObservableCollection<QuestionIntrospectionPage>() {
                new QuestionIntrospectionPage("Ich kann phänologische Entwicklungsstadien von Feldfrüchten zuverlässig erkennen.")};
            }
            this.QuestionItem = Items[0];
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

        Dictionary<RadioButton, int> RadioButtonIndex;

        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            var selectedRadioButton = RadioButtonIndex.Keys.FirstOrDefault(r => r.IsChecked);
            if (selectedRadioButton == null)
                return;
            AnswerItem = new AnswerIntrospetionPage(QuestionItem, RadioButtonIndex[selectedRadioButton]);

            QuestionItem = new QuestionIntrospectionPage("Ich kann ... zuverlässig erkennen.");

            Button1.IsChecked = false;
            Button2.IsChecked = false;
            Button3.IsChecked = false;
            Button4.IsChecked = false;
            Button5.IsChecked = false;
        }

        void OnAbbrechenButtonClicked(object sender, EventArgs e)
        {

        }
    }
}