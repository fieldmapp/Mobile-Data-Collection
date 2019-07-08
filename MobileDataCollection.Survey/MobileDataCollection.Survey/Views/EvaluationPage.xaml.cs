using MobileDataCollection.Survey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileDataCollection.Survey.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EvaluationPage : ContentPage
    {
        public static readonly BindableProperty EvaluationItemProperty = BindableProperty.Create(nameof(EvaluationItem),
            typeof(EvaluationItem), typeof(EvaluationPage), null);

        private EvaluationItem EvaluationItem
        {
            get => (EvaluationItem)GetValue(EvaluationItemProperty);
            set => SetValue(EvaluationItemProperty, value);
        }

        public EvaluationPage(EvaluationItem evalItem)
        {
            InitializeComponent();
            EvaluationItem = evalItem;
            PercentBar.BindingContext = this;
            PercentLabel.BindingContext = this;
        }
        void DetailsClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EvaluationDetailsPage(EvaluationItem.PercentEasy,EvaluationItem.PercentMedium,EvaluationItem.PercentHard));
        }
    }
}