using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
	public class QuestionDoubleSliderPage
    {
        /// <summary>
        /// Defines the maximum valid value of <see cref="Level"/>
        /// </summary>
        const int HighestQuestionDifficulty = 3;

        /// <summary>
        /// Number of available questions (Remove?)
        /// </summary>
        public int AnswersNeeded { get; set; } = 4;
        
        /// <summary>
        /// Question text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Picture URI
        /// </summary>
        public string PictureAddress { get; set; }

        /// <summary>
        /// Correct answer for SliderA (Bodenbedeckung)
        /// </summary>
        public int RightAnswerA { get; set; }
        
        /// <summary>
        /// Correct answer for SliderB (Grüne Pflanzenanteile)
        /// </summary>
        public int RightAnswerB { get; set; }

        /// <summary>
        /// Level of the question. Must be in range 1 to <see cref="HighestQuestionDifficulty"/> (inclusive)
        /// </summary>
        public int Level
        {
            get => _level;
            set
            {
                if (value > HighestQuestionDifficulty)
                    throw new NotImplementedException($"{nameof(value)} must be at most {nameof(HighestQuestionDifficulty)}={HighestQuestionDifficulty}");
                if (value < 1)
                    throw new NotImplementedException($"{nameof(value)} must be at least 1");
                _level = value;
            }
        }

        private int _level;

        /// <summary>
        /// Reflects if the user has allready submitted an answer to the question
        /// </summary>
        public bool Answered { get; set; }

        /// <summary>
        /// Answer given by the user. Check <see cref="Answered"/> to see if user has submitted the answer first.
        /// </summary>
        public int Result { get; set; }
        
        public QuestionDoubleSliderPage(string pictureAddress, int answerA, int answerB, int level)
        {
            Text = "Schätzen Sie den Grad der Bedeckung des Bodens durch Pflanzen (A) und den Anteil grüner Pflanzenbestandteile (B) ein.";
            PictureAddress = pictureAddress;
            RightAnswerA = answerA;
            RightAnswerB = answerB;
            Level = level;
            Answered = false;
        }

    }
}