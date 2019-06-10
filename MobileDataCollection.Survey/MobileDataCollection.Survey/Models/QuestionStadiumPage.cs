using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace MobileDataCollection.Survey.Models
{
    class QuestionStadiumPage
    {
        //TODO: Add List with new class (eg FruitTypeSubItem)
        //TODO: Think of a way to store correct answer

        /// <summary>
        /// Contains all possible stadiums
        /// </summary>
        List<StadiumSubItem> StadiumSubItems { get; set; } = new List<StadiumSubItem>();
        ObservableCollection<Plant> TestCollection2 = new ObservableCollection<Plant>();
    }
}
