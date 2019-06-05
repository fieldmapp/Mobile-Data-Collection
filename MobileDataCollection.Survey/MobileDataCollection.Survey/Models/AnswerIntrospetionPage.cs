using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    class AnswerIntrospetionPage
    {
        /// <summary>
        /// Represents the index of the selected Answer (in inclusive range 1 - 4)
        /// </summary>
        public int SelectedAnswer { get; set; }

        /// <summary>
        /// Is a reference to the belonging Question
        /// </summary>
        public QuestionIntrospectionPage Question { get; set; }

        public AnswerIntrospetionPage(QuestionIntrospectionPage question)
        {
            Question = question;
        }
    }
}
