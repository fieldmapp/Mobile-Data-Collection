using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public enum SurveyMenuItemType
    {
        Introspection,
        ImageChecker,
        DoubleSlider,
        Stadium
    }
    public class SurveyMenuItem
    {
        public SurveyMenuItemType Id { get; set; }

        public string ChapterName { get; set; }

        public int AnswersNeeded { get; set; }

        public int AnswersGiven { get; set; }

        public bool Unlocked { get; set; }

        public string ProgressString => $"{AnswersGiven}/{AnswersNeeded}";

        public Color BackgroundColor { get; set; }
    }
}
