//Main contributors: Maya Koehnen
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.ProfilingSharedModule.Models
{
    /// <summary>
    /// Class EvaluationItem, which defines the properties for an element displayed on the main evaluation page
    /// It has an ProfilingMenuItemType Id, defining the assigned question category and four integer values for
    /// the displayed results. Other properties are for the defining the design: Name of the question category, 
    /// bar color etc.
    /// </summary>
    public class EvaluationItem : BindableObject
    {
        public static readonly BindableProperty PercentProperty = BindableProperty.Create(nameof(Percent), typeof(int), typeof(EvaluationItem), 0);
        public static readonly BindableProperty PercentEasyProperty = BindableProperty.Create(nameof(PercentEasy), typeof(int), typeof(EvaluationItem), 0);
        public static readonly BindableProperty PercentMediumProperty = BindableProperty.Create(nameof(PercentMedium), typeof(int), typeof(EvaluationItem), 0);
        public static readonly BindableProperty PercentHardProperty = BindableProperty.Create(nameof(PercentHard), typeof(int), typeof(EvaluationItem), 0);
        public static readonly BindableProperty CategoryNameProperty = BindableProperty.Create(nameof(CategoryName), typeof(string), typeof(EvaluationItem), string.Empty);
        public static readonly BindableProperty BarColorProperty = BindableProperty.Create(nameof(BarColor), typeof(Color), typeof(EvaluationItem), Color.Black);
        public static readonly BindableProperty PercentBarValueProperty = BindableProperty.Create(nameof(PercentBarValue), typeof(double), typeof(EvaluationItem), 0d);
        public static readonly BindableProperty PercentLabelTextProperty = BindableProperty.Create(nameof(PercentLabelText), typeof(string), typeof(EvaluationItem), string.Empty);


        /// Overall result for the question category as percentage
        public int Percent
        {
            get => (int)GetValue(PercentProperty);
            set => SetValue(PercentProperty, value);
        }
        /// Result for the question with difficulty level "easy" as percentage
        public int PercentEasy
        {
            get => (int)GetValue(PercentEasyProperty);
            set => SetValue(PercentEasyProperty, value);
        }
        /// Result for the question with difficulty level "medium" as percentage
        public int PercentMedium
        {
            get => (int)GetValue(PercentMediumProperty);
            set => SetValue(PercentMediumProperty, value);
        }
        /// Result for the question with difficulty level "hard" as percentage
        public int PercentHard
        {
            get => (int)GetValue(PercentHardProperty);
            set => SetValue(PercentHardProperty, value);
        }
        /// Name of the question category to display as text 
        public string CategoryName
        {
            get => (string)GetValue(CategoryNameProperty);
            set => SetValue(CategoryNameProperty, value);
        }
        /// Color of the progress bar, which is the grafical representation of the result, 
        /// will be changed depending on the result
        public Color BarColor
        {
            get => (Color)GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }
        ///  Result as value for the progressbar
        public double PercentBarValue
        {
            get => (double)GetValue(PercentBarValueProperty);
            set => SetValue(PercentBarValueProperty, value);
        }
        /// Result (percentage) displayed as text
        public string PercentLabelText
        {
            get => (string)GetValue(PercentLabelTextProperty);
            set => SetValue(PercentLabelTextProperty, value);
        }

        /// <summary>
        /// Constructor for the EvaluationItem
        /// </summary>
        /// <param name="catname">String representing the name of the category</param>
        /// <param name="percent">Overall result</param>
        /// <param name="percentEasy">Result for questions with difficulty level "easy"</param>
        /// <param name="percentMedium">Result for questions with difficulty level "medium"</param>
        /// <param name="percentHard">Result for questions with difficulty level "hard"</param>
        public EvaluationItem(string catname, int percent, int percentEasy, int percentMedium, int percentHard)
        {
            ///Defining the properties of the EvaluatioItem
            CategoryName = catname;
            Percent = percent;
            PercentEasy = percentEasy;
            PercentMedium = percentMedium;
            PercentHard = percentHard;
            ///Setting the grafical elements
            PercentBarValue = (double)percent / 100;
            PercentLabelText = $"{percent}%";
            if (percent <= 33) BarColor = Color.LightSalmon;
            else if (percent <= 66) BarColor = Color.Gold;
            else BarColor = Color.DarkSeaGreen;
            ///Checking wether there is an result available for this category (if not, the result is negative) and if that
            /// is the case, defining how it shall be displayed
            if (percent < 0)
            {
                Percent = 0;
                PercentLabelText = $"-   ";
            }
        }
    }
    
}
