using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class EvaluationItem
    {
            public SurveyMenuItemType Id { get; set; }
            public int Percent { get; set; }
            public int PercentEasy { get; set; }
            public int PercentMedium { get; set; }
            public int PercentHard { get; set; }
            public string CatName { get; set; }
            public Color BarColor { get; set; }
        public string PercentLabelText { get; set; }
        public double PercentBarValue { get; set; }
        public EvaluationItem(string catname, int Percent, int PercentEasy, int PercentMedium, int PercentHard)
        {
            this.CatName = catname;
            this.Percent = Percent;
            this.PercentBarValue = (double)Percent / 100;
            PercentLabelText = $"{Percent}%";
            if (Percent < 0)
            {
                this.Percent = 0;
                PercentLabelText = $"-   ";
            }
            this.PercentEasy = PercentEasy;
            this.PercentMedium = PercentMedium;
            this.PercentHard = PercentHard;
            if (Percent <= 33)  this.BarColor = Color.LightSalmon;
            else if (Percent <= 66)  this.BarColor = Color.Gold;
            else this.BarColor = Color.DarkSeaGreen;
            
        }
    }
    
}
