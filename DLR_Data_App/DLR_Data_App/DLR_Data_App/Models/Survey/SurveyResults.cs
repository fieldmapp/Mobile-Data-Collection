using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models.Survey
{
    public class SurveyResults
    {
        public string UserId { get; set; }
        public List<SurveyResult> Results { get; set; } = new List<SurveyResult>();
        public int ProjectsFilledSinceLastSurveyCompletion = 0;
    }

    public class SurveyResult
    {
        public DateTime TimeStamp { get; set; }
        public Dictionary<string, List<IUserAnswer>> UserAnswers { get; set; } = new Dictionary<string, List<IUserAnswer>>();
    }
}
