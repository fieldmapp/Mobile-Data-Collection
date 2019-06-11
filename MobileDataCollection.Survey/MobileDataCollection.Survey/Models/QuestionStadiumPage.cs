using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    class QuestionStadiumPage : IQuestionContent
    {
        //TODO: Add List with new class (eg FruitTypeSubItem)
        //TODO: Think of a way to store correct answer

        /// <summary>
        /// Contains all possible stadiums
        /// </summary>
        List<StadiumSubItem> StadiumSubItems { get; set; } = new List<StadiumSubItem>();
    }
}
