using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    class AnswerImageCheckerPage
    {
        /// <summary>
        /// Intern Id for Answers of this Type, corrosponds to same number as in QuestionImageCheckerPage
        /// </summary>
        [PrimaryKey]
        public int InternId { get; set; }
        /// <summary>
        /// Reflects whether Image 1 is selected
        /// </summary>
        public int Image1Selected { get; set; }

        /// <summary>
        /// Reflects whether Image 2 is selected
        /// </summary>
        public int Image2Selected { get; set; }

        /// <summary>
        /// Reflects whether Image 3 is selected
        /// </summary>
        public int Image3Selected { get; set; }

        /// <summary>
        /// Reflects whether Image 4 is selected
        /// </summary>
        public int Image4Selected { get; set; }

        /// <summary>
        /// Is a reference to the belonging Question
        /// </summary>

        public AnswerImageCheckerPage(int Id,int selected1, int selected2, int selected3, int selected4)
        {
            InternId = Id;
            Image1Selected = selected1;
            Image2Selected = selected2;
            Image3Selected = selected3;
            Image4Selected = selected4;
        }

        public AnswerImageCheckerPage()
        {
            
        }
    }
}
