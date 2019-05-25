using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
	public class QuestionDoubleSliderPage
    {
        public SurveyMenuItemType Id { get; set; }
        public string Text { get; set; }
        public string[] PictureAdresses { get; set; }
        public int[] RightAnswers { get; set; }

    }
}