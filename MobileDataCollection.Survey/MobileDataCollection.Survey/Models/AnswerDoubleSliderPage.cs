using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    class AnswerDoubleSliderPage
    {
        /// <summary>
        /// Answer given by the user. Check <see cref="Answered"/> to see if user has submitted the answer first.
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Is a reference to the belonging Question
        /// </summary>
        public QuestionDoubleSliderPage Question { get; set; }

        public AnswerDoubleSliderPage(QuestionDoubleSliderPage question)
        {
            Question = question;
        }
    }
}
