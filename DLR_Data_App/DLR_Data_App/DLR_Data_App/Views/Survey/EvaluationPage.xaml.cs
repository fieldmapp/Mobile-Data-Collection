//Main contributors: Maya Koehnen
using DLR_Data_App.Models.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Survey
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EvaluationPage : ContentPage
    {
        public static readonly BindableProperty PercentLabelProperty = BindableProperty.Create(nameof(PercentLabelText),
            typeof(String), typeof(EvaluationPage), "100%", BindingMode.OneWay);

        public String PercentLabelText
        {
            get { return (String)GetValue(PercentLabelProperty); }
            set { SetValue(PercentLabelProperty, value); }
        }

        public static readonly BindableProperty PercentProperty = BindableProperty.Create(nameof(PercentBarValue),
            typeof(double), typeof(EvaluationPage), 0.2, BindingMode.OneWay);
        public double PercentBarValue
        {
            get { return (double)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        /* public static readonly BindableProperty BarColorProperty = BindableProperty.Create(nameof(BarColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkSeaGreen, BindingMode.OneWay);
       public Color BarColor
        {
            get { return (Color)GetValue(BarColorProperty); }
            set { SetValue(BarColorProperty, value); }
        }*/
        public static readonly BindableProperty ProgressColorProperty = BindableProperty.Create(nameof(ProgressColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkOliveGreen, BindingMode.OneWay);
        public Color ProgressColor
        {
            get { return (Color)GetValue(ProgressColorProperty); }
            set { SetValue(ProgressColorProperty, value); }
        }

        private EvaluationItem EvaluationItem;
        public event EventHandler PageFinished;

        public EvaluationPage(EvaluationItem evalItem)
        {
            InitializeComponent();
            EvaluationItem = evalItem;
            PercentBarValue = (double)evalItem.Percent / 100;
            PercentBar.BindingContext = this;
            ProgressColor = evalItem.BarColor;
            PercentLabel.BindingContext = this;
            PercentLabelText = $"{evalItem.Percent}%";
        }
        void DetailsClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EvaluationDetailsPage(EvaluationItem.PercentEasy,EvaluationItem.PercentMedium,EvaluationItem.PercentHard));
        }

        protected override bool OnBackButtonPressed()
        {
            PageFinished?.Invoke(this, null);
            return true;
        }
    }
}