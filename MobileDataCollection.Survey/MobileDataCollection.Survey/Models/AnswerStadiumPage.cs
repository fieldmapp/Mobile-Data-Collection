using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    class AnswerStadiumPage : IUserAnswer
    {
        /// <summary>
        /// Selected fruit type
        /// </summary>
        string AnswerFruitType { get; set; }

        /// <summary>
        /// Selected stadium
        /// </summary>
        string AnswerStadium { get; set; }

        /// <summary>
        /// Is a reference to the belonging Question
        /// </summary>
        QuestionStadiumPage Question { get; set; }

        public AnswerStadiumPage(QuestionStadiumPage question)
        {
            Question = question;
        }
    }
}
