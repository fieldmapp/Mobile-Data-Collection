using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    /// <summary>
    /// Class EvaluationItem, which defines the properties for an element displayed on the main evaluation page
    /// It has an SurveyMenuItemType Id, defining the assigned question category and four integer values for
    /// the displayed results. Other properties are for the defining the design: Name of the question category, 
    /// bar color etc.
    /// </summary>
    public class EvaluationItem
    {
        ///SurveyMenuItemType Id, defining the assigned question category
        public SurveyMenuItemType Id { get; set; }
        /// Overall result for the question category as percentage
        public int Percent { get; set; }
        /// Result for the question with difficulty level "easy" as percentage
        public int PercentEasy { get; set; }
        /// Result for the question with difficulty level "medium" as percentage
        public int PercentMedium { get; set; }
        /// Result for the question with difficulty level "hard" as percentage
        public int PercentHard { get; set; }
        /// Name of the question category to display as text 
        public string CatName { get; set; }
        /// Color of the progress bar, which is the grafical representation of the result, 
        /// will be changed depending on the result
        public Color BarColor { get; set; }
        ///  Result as value for the progressbar
        public double PercentBarValue { get; set; }
        /// Result (percentage) displayed as text
        public string PercentLabelText { get; set; }
        /// <summary>
        /// Constructor for the EvaluationItem
        /// </summary>
        /// <param name="catname">String representing the name of the category</param>
        /// <param name="Percent">Overall result</param>
        /// <param name="PercentEasy">Result for questions with difficulty level "easy"</param>
        /// <param name="PercentMedium">Result for questions with difficulty level "medium"</param>
        /// <param name="PercentHard">Result for questions with difficulty level "hard"</param>
        public EvaluationItem(string catname, int Percent, int PercentEasy, int PercentMedium, int PercentHard)
        {
            ///Defining the properties of the EvaluatioItem
            this.CatName = catname;
            this.Percent = Percent;
            this.PercentEasy = PercentEasy;
            this.PercentMedium = PercentMedium;
            this.PercentHard = PercentHard;
            ///Setting the grafical elements
            this.PercentBarValue = (double)Percent / 100;
            PercentLabelText = $"{Percent}%";
            if (Percent <= 33) this.BarColor = Color.LightSalmon;
            else if (Percent <= 66) this.BarColor = Color.Gold;
            else this.BarColor = Color.DarkSeaGreen;
            ///Checking wether there is an result available for this category (if not, the result is negative) and if that
            /// is the case, defining how it shall be displayed
            if (Percent < 0)
            {
                this.Percent = 0;
                PercentLabelText = $"-   ";
            }
        }
    }
    
}
