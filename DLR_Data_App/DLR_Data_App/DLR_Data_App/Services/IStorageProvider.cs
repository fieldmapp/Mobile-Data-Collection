using DLR_Data_App.Models.Profiling;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Services
{
    public interface IStorageProvider
    {
        Dictionary<string, List<IQuestionContent>> LoadQuestions();
        Dictionary<string, string> LoadTranslations();
        ProfilingData LoadProfilingData();
        ProfilingResults LoadAnswers(string userId);
        void SaveAnswers(ProfilingResults results);
        void ExportAnswers(ProfilingResults results);
        void ExportDatabase(string content);
    }
}
