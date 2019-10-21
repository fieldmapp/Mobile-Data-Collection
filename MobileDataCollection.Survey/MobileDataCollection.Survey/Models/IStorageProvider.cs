using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    public interface IStorageProvider
    {
        Dictionary<string, List<IQuestionContent>> LoadQuestions();
        List<SurveyMenuItem> LoadSurveyMenuItems();
        Dictionary<string, List<IUserAnswer>> LoadAnswers();
        void SaveAnswers(Dictionary<string, List<IUserAnswer>> answers);
        void ExportAnswers(Dictionary<string, List<IUserAnswer>> answers);
    }
}
