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
        public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof(Item), 
            typeof(QuestionIntrospectionPage), typeof(IntrospectionPage), new QuestionIntrospectionPage("demo"), 
            BindingMode.OneWay);

        public QuestionIntrospectionPage Item
        {
            get { return (QuestionIntrospectionPage)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public IntrospectionPage()
		{
            InitializeComponent();
            QuestionLabel.BindingContext = this;
            Item = new QuestionIntrospectionPage("Ich kann eine Sorte von Feldfrüchten zuverlässig erkennen.");
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