using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    class QuestionImageCheckerPage
    {
        public int NumberOfPossibleAnswers { get; set; }
        public int NumberOfCorrectAnswers { get; set; }
        public bool[] StandardSolution { get; set; }
        public bool[] GivenSolution { get; set; }
        public bool[] CorrectGivenSolution { get; set; }
        public string[] SourcesBigPictures { get; set; }
        public string[] SourcesSmallPictures { get; set; }
    }
}
