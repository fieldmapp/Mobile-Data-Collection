using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
	public class QuestionDoubleSliderPage
    {
        public int AnswersNeeded { get; set; } = 4;
        public string Text { get; set; }
        public string PictureAddress { get; set; }
        public int RightAnswerA { get; set; }
        public int RightAnswerB { get; set; }
        
        public QuestionDoubleSliderPage(string PictureAddress, int AnswerA, int AnswerB)
        {
            this.Text = "Schätzen Sie den Grad der Bedeckung des Bodens durch Pflanzen (A) und den Anteil grüner Pflanzenbestandteile (B) ein.";
            this.PictureAddress = PictureAddress;
            this.RightAnswerA = AnswerA;
            this.RightAnswerB = AnswerB;
        }

    }
}