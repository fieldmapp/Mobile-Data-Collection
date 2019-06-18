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
    public partial class EvaluationDetailsPage : ContentPage
    {
        public static readonly BindableProperty PercentEasyLabelProperty = BindableProperty.Create(nameof(PercentEasyLabelText),
            typeof(String), typeof(EvaluationPage), "100%", BindingMode.OneWay);
        public static readonly BindableProperty PercentMediumLabelProperty = BindableProperty.Create(nameof(PercentMediumLabelText),
            typeof(String), typeof(EvaluationPage), "100%", BindingMode.OneWay);
        public static readonly BindableProperty PercentHardLabelProperty = BindableProperty.Create(nameof(PercentHardLabelText),
            typeof(String), typeof(EvaluationPage), "100%", BindingMode.OneWay);
        public String PercentEasyLabelText {
            get { return (String)GetValue(PercentEasyLabelProperty); }
            set { SetValue(PercentEasyLabelProperty, value); }  }
        public String PercentMediumLabelText {
            get { return (String)GetValue(PercentMediumLabelProperty); }
            set { SetValue(PercentMediumLabelProperty, value); }  }
        public String PercentHardLabelText {
            get { return (String)GetValue(PercentHardLabelProperty); }
            set { SetValue(PercentHardLabelProperty, value); }
        }
        public static readonly BindableProperty PercentEasyProperty = BindableProperty.Create(nameof(PercentEasyBarValue),
            typeof(double), typeof(EvaluationPage), 0.2, BindingMode.OneWay);
        public static readonly BindableProperty PercentMediumProperty = BindableProperty.Create(nameof(PercentMediumBarValue),
            typeof(double), typeof(EvaluationPage), 0.2, BindingMode.OneWay);
        public static readonly BindableProperty PercentHardProperty = BindableProperty.Create(nameof(PercentHardBarValue),
            typeof(double), typeof(EvaluationPage), 0.2, BindingMode.OneWay);
        public double PercentEasyBarValue {
            get { return (double)GetValue(PercentEasyProperty); }
            set { SetValue(PercentEasyProperty, value); }   }
        public double PercentMediumBarValue {
            get { return (double)GetValue(PercentMediumProperty); }
            set { SetValue(PercentMediumProperty, value); }   }
        public double PercentHardBarValue {
            get { return (double)GetValue(PercentHardProperty); }
            set { SetValue(PercentHardProperty, value); }   }

        public static readonly BindableProperty BarEasyColorProperty = BindableProperty.Create(nameof(BarEasyColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkSeaGreen, BindingMode.OneWay);
        public Color BarEasyColor
        {   get { return (Color)GetValue(BarEasyColorProperty); }
            set { SetValue(BarEasyColorProperty, value); }  }
        public static readonly BindableProperty ProgressEasyColorProperty = BindableProperty.Create(nameof(ProgressEasyColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkOliveGreen, BindingMode.OneWay);
        public Color ProgressEasyColor
        {   get { return (Color)GetValue(ProgressEasyColorProperty); }
            set { SetValue(ProgressEasyColorProperty, value); } }
        public static readonly BindableProperty BarMediumColorProperty = BindableProperty.Create(nameof(BarMediumColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkSeaGreen, BindingMode.OneWay);
        public Color BarMediumColor
        {
            get { return (Color)GetValue(BarMediumColorProperty); }
            set { SetValue(BarMediumColorProperty, value); }
        }
        public static readonly BindableProperty ProgressMediumColorProperty = BindableProperty.Create(nameof(ProgressMediumColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkOliveGreen, BindingMode.OneWay);
        public Color ProgressMediumColor
        {
            get { return (Color)GetValue(ProgressMediumColorProperty); }
            set { SetValue(ProgressMediumColorProperty, value); }
        }
        public static readonly BindableProperty BarHardColorProperty = BindableProperty.Create(nameof(BarHardColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkSeaGreen, BindingMode.OneWay);
        public Color BarHardColor
        {
            get { return (Color)GetValue(BarHardColorProperty); }
            set { SetValue(BarHardColorProperty, value); }
        }
        public static readonly BindableProperty ProgressHardColorProperty = BindableProperty.Create(nameof(ProgressHardColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkOliveGreen, BindingMode.OneWay);
        public Color ProgressHardColor
        {
            get { return (Color)GetValue(ProgressHardColorProperty); }
            set { SetValue(ProgressHardColorProperty, value); }
        }

        public EvaluationDetailsPage(int ResultEasy, int ResultMedium, int ResultHard)
        {
            InitializeComponent();
            if (ResultEasy <= 33)
            {
                BarEasyColor = Color.PeachPuff;
                ProgressEasyColor = Color.LightSalmon;
            }
            else if (ResultEasy <= 66)
            {
                BarEasyColor = Color.Khaki;
                ProgressEasyColor = Color.Gold;
            }
            else
            {
                BarEasyColor = Color.DarkSeaGreen;
                ProgressEasyColor = Color.DarkOliveGreen;
            }
            if (ResultMedium <= 33)
            {
                BarMediumColor = Color.PeachPuff;
                ProgressMediumColor = Color.LightSalmon;
            }
            else if (ResultMedium <= 66)
            {
                BarMediumColor = Color.Khaki;
                ProgressMediumColor = Color.Gold;
            }
            else
            {
                BarMediumColor = Color.DarkSeaGreen;
                ProgressMediumColor = Color.DarkOliveGreen;
            }
            if (ResultHard <= 33)
            {
                BarHardColor = Color.PeachPuff;
                ProgressHardColor = Color.LightSalmon;
            }
            else if (ResultHard <= 66)
            {
                BarHardColor = Color.Khaki;
                ProgressHardColor = Color.Gold;
            }
            else
            {
                BarHardColor = Color.DarkSeaGreen;
                ProgressHardColor = Color.DarkOliveGreen;
            }
            this.PercentEasyBarValue = (double)ResultEasy / 100;
            PercentEasyBar.BindingContext = this;
            this.PercentMediumBarValue = (double)ResultMedium / 100;
            PercentMediumBar.BindingContext = this;
            this.PercentHardBarValue = (double)ResultHard / 100;
            PercentHardBar.BindingContext = this;
            PercentEasyLabel.BindingContext = this;
            PercentEasyLabelText = $"{ResultEasy}%";
            PercentMediumLabel.BindingContext = this;
            PercentMediumLabelText = $"{ResultMedium}%";
            PercentHardLabel.BindingContext = this;
            PercentHardLabelText = $"{ResultHard}%";
        }
    }
}