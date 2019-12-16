using DLR_Data_App.Models.Survey;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Services
{
    public interface IStorageProvider
    {
        Dictionary<string, List<IQuestionContent>> LoadQuestions();
        Dictionary<string, string> LoadTranslations();
        SurveyData LoadSurveyData();
        SurveyResults LoadAnswers(string userId);
        void SaveAnswers(SurveyResults results);
        void ExportAnswers(SurveyResults results);
        void ExportDatabase(string content);
    }
}
