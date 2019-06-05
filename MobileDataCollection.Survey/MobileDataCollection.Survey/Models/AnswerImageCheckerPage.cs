using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    class AnswerImageCheckerPage
    {
        /// <summary>
        /// Reflects whether Image 1 is selected
        /// </summary>
        public bool Image1Selected { get; set; }

        /// <summary>
        /// Reflects whether Image 2 is selected
        /// </summary>
        public bool Image2Selected { get; set; }

        /// <summary>
        /// Reflects whether Image 3 is selected
        /// </summary>
        public bool Image3Selected { get; set; }

        /// <summary>
        /// Reflects whether Image 4 is selected
        /// </summary>
        public bool Image4Selected { get; set; }

        /// <summary>
        /// Is a reference to the belonging Question
        /// </summary>
        public QuestionImageCheckerPage Question { get; set; }

        public AnswerImageCheckerPage(QuestionImageCheckerPage question)
        {
            Question = question;
        }
    }
}
