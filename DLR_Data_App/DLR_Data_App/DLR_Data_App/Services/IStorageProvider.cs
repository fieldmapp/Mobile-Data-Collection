using DLR_Data_App.Models.Survey;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Services
{
    public interface IStorageProvider
    {
        Dictionary<string, List<IQuestionContent>> LoadQuestions();
        List<SurveyMenuItem> LoadSurveyMenuItems();
        Dictionary<string, List<IUserAnswer>> LoadAnswers(string userId);
        void SaveAnswers(Dictionary<string, List<IUserAnswer>> answers, string userId);
        void ExportAnswers(Dictionary<string, List<IUserAnswer>> answers, string userId);
        void ExportDatabase(string content);
    }
}
