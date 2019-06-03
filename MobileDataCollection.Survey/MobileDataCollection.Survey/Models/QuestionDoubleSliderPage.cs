using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
	public class QuestionDoubleSliderPage
    {
        //Attributes of a Double-Slider Question
        public int AnswersNeeded { get; set; } = 4; //Number of available Questions (Remove?)
        //Question text
        public string Text { get; set; }
        //Location address of the picture
        public string PictureAddress { get; set; }
        //Correct answer for SliderA (Bodenbedeckung)
        public int RightAnswerA { get; set; }
        //Correct answer for SliderB (Grüne Pflanzenanteile)
        public int RightAnswerB { get; set; }
        //Shows wether the question has already been submitted
        public Boolean Answered { get; set; }
        //Level of the Question (1-3)
        public int Level { get; set; }
        //Result of the given Answers (Percentage/Diff/... to the right answers)
        public int Result { get; set; }
        
        public QuestionDoubleSliderPage(string PictureAddress, int AnswerA, int AnswerB, int level)
        {
            this.Text = "Schätzen Sie den Grad der Bedeckung des Bodens durch Pflanzen (A) und den Anteil grüner Pflanzenbestandteile (B) ein.";
            this.PictureAddress = PictureAddress;
            this.RightAnswerA = AnswerA;
            this.RightAnswerB = AnswerB;
            this.Level = level;
            this.Answered = false;
        }

    }
}