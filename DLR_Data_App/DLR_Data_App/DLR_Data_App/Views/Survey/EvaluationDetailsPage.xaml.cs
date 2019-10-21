//Main contributors: Maya Koehnen
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
    public partial class EvaluationDetailsPage : ContentPage
    {
        /// <summary>
        /// Properties and for designelements
        /// Results for easy questions
        /// Label which displays result als text
        public static readonly BindableProperty PercentEasyLabelProperty = BindableProperty.Create(nameof(PercentEasyLabelText),
            typeof(String), typeof(EvaluationPage), "100%", BindingMode.OneWay);
        public String PercentEasyLabelText
        {
            get { return (String)GetValue(PercentEasyLabelProperty); }
            set { SetValue(PercentEasyLabelProperty, value); }
        }

        /// Percent for the progressbar as grafical representation of the result
        public static readonly BindableProperty PercentEasyProperty = BindableProperty.Create(nameof(PercentEasyBarValue),
            typeof(double), typeof(EvaluationPage), 0.2, BindingMode.OneWay);
        public double PercentEasyBarValue
        {
            get { return (double)GetValue(PercentEasyProperty); }
            set { SetValue(PercentEasyProperty, value); }
        }
        /// Color of the progressbar, depending on how good the result ist
        public static readonly BindableProperty ProgressEasyColorProperty = BindableProperty.Create(nameof(ProgressEasyColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkOliveGreen, BindingMode.OneWay);
        public Color ProgressEasyColor
        {
            get { return (Color)GetValue(ProgressEasyColorProperty); }
            set { SetValue(ProgressEasyColorProperty, value); }
        }
        /// Result for medium questions
        /// Label which displays result als text
        public static readonly BindableProperty PercentMediumLabelProperty = BindableProperty.Create(nameof(PercentMediumLabelText),
            typeof(String), typeof(EvaluationPage), "100%", BindingMode.OneWay);
        public String PercentMediumLabelText
        {
            get { return (String)GetValue(PercentMediumLabelProperty); }
            set { SetValue(PercentMediumLabelProperty, value); }
        }
        /// Percent for the progressbar as grafical representation of the result
        public static readonly BindableProperty PercentMediumProperty = BindableProperty.Create(nameof(PercentMediumBarValue),
            typeof(double), typeof(EvaluationPage), 0.2, BindingMode.OneWay);
        public double PercentMediumBarValue
        {
            get { return (double)GetValue(PercentMediumProperty); }
            set { SetValue(PercentMediumProperty, value); }
        }
        ///  Color of the progressbar, depending on how good the result ist
        public static readonly BindableProperty ProgressMediumColorProperty = BindableProperty.Create(nameof(ProgressMediumColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkOliveGreen, BindingMode.OneWay);
        public Color ProgressMediumColor
        {
            get { return (Color)GetValue(ProgressMediumColorProperty); }
            set { SetValue(ProgressMediumColorProperty, value); }
        }

        /// Result for hard questions
        /// Label which displays result als text
        public static readonly BindableProperty PercentHardLabelProperty = BindableProperty.Create(nameof(PercentHardLabelText),
            typeof(String), typeof(EvaluationPage), "100%", BindingMode.OneWay);
        public String PercentHardLabelText
        {
            get { return (String)GetValue(PercentHardLabelProperty); }
            set { SetValue(PercentHardLabelProperty, value); }
        }
        /// Percent for the progressbar as grafical representation of the result
        public static readonly BindableProperty PercentHardProperty = BindableProperty.Create(nameof(PercentHardBarValue),
            typeof(double), typeof(EvaluationPage), 0.2, BindingMode.OneWay);
        public double PercentHardBarValue
        {
            get { return (double)GetValue(PercentHardProperty); }
            set { SetValue(PercentHardProperty, value); }
        }
        ///  Color of the progressbar, depending on how good the result ist
        public static readonly BindableProperty ProgressHardColorProperty = BindableProperty.Create(nameof(ProgressHardColor),
            typeof(Color), typeof(EvaluationPage), Color.DarkOliveGreen, BindingMode.OneWay);
        public Color ProgressHardColor
        {
            get { return (Color)GetValue(ProgressHardColorProperty); }
            set { SetValue(ProgressHardColorProperty, value); }
        }

        /// <param name="ResultEasy">Result the user has achived for question with the level "easy"</param>
        /// <param name="ResultMedium">Result the user has achived for question with the level "medium"</param>
        /// <param name="ResultHard">Result the user has achived for question with the level "hard"</param>
        public EvaluationDetailsPage(int ResultEasy, int ResultMedium, int ResultHard)
        {
            InitializeComponent();
            ///Defining the design properties for the grafical representation of the achived percentages
            ///Easy Questions
            ///Color of the progressbar
            if (ResultEasy <= 33)ProgressEasyColor = Color.LightSalmon;
            else if (ResultEasy <= 66) ProgressEasyColor = Color.Gold;
            else  ProgressEasyColor = Color.DarkSeaGreen;
            ///Percentages of the Progessbars as grafical display of the achived percentages
            this.PercentEasyBarValue = (double)ResultEasy / 100;
            /// Defining the Labeltext (display of the percentage as text)
            PercentEasyLabelText = $"{ResultEasy}%";
            /// Checking wether the user has answered Questions in this levels and 
            /// if that is not the case (negative Percentage) adjusting the grafics
            if (ResultEasy < 0)
            {
                this.PercentEasyBarValue = 0;
                this.PercentEasyLabelText = $"-";
            }
            ///Setting the binding contexts
            PercentEasyLabel.BindingContext = this;
            PercentEasyBar.BindingContext = this;
            ///Medium Questions
            ///Color of the progressbar
            if (ResultMedium <= 33)  ProgressMediumColor = Color.LightSalmon;
            else if (ResultMedium <= 66) ProgressMediumColor = Color.Gold;
            else ProgressMediumColor = Color.DarkSeaGreen;
            ///Percentages of the Progessbars as grafical display of the achived percentages
            this.PercentMediumBarValue = (double)ResultMedium / 100;
            /// Defining the Labeltext (display of the percentage as text)
            PercentMediumLabelText = $"{ResultMedium}%";
            /// Checking wether the user has answered Questions in this levels and 
            /// if that is not the case (negative Percentage) adjusting the grafics
            if (ResultMedium < 0)
            {
                this.PercentMediumBarValue = 0;
                this.PercentMediumLabelText = $"-";
            }
            ///Setting the binding contexts
            PercentMediumBar.BindingContext = this;
            PercentMediumLabel.BindingContext = this;
            ///Hard Question
            ///Color of the progressbar
            if (ResultHard <= 33) ProgressHardColor = Color.LightSalmon;
            else if (ResultHard <= 66) ProgressHardColor = Color.Gold;
            else ProgressHardColor = Color.DarkSeaGreen;
            ///Percentages of the Progessbars as grafical display of the achived percentages
            this.PercentHardBarValue = (double)ResultHard / 100;
            /// Defining the Labeltext (display of the percentage as text)
            PercentHardLabelText = $"{ResultHard}%";
            /// Checking wether the user has answered Questions in this levels and 
            /// if that is not the case (negative Percentage) adjusting the grafics
            if (ResultHard < 0)
            {
                this.PercentHardBarValue = 0;
                this.PercentHardLabelText = $"-";
            }
            ///Setting the binding contexts
            PercentHardBar.BindingContext = this;
            PercentHardLabel.BindingContext = this;
        }
    }
}