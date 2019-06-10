using MobileDataCollection.Survey.Controls;
using MobileDataCollection.Survey.Models;
using System;
using System.Collections.Generic;
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
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem), typeof(AnswerIntrospetionPage), typeof(IntrospectionPage), new AnswerIntrospetionPage(new QuestionIntrospectionPage("demo"), 1), BindingMode.OneWay);

        public QuestionIntrospectionPage QuestionItem
        {
            get { return (QuestionIntrospectionPage)GetValue(QuestionItemProperty); }
            set { SetValue(QuestionItemProperty, value); }
        }

        public AnswerIntrospetionPage AnswerItem
        {
            get { return (AnswerIntrospetionPage)GetValue(AnswerItemProperty); }
            set { SetValue(AnswerItemProperty, value); }
        }

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
            var button = (sender as RadioButton);
            if (button != null && RadioButtonIndex.TryGetValue(button, out int index))
            {
                AnswerItem = new AnswerIntrospetionPage(QuestionItem, index);
            }
            else
                throw new NotSupportedException("sender is either not a RadioButton or not yet supported");

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