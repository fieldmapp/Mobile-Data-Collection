using DLR_Data_App.Models.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLR_Data_App.Services
{
    public class MockQuestionProvider : IStorageProvider
    {
        public void ExportAnswers(ProfilingResults results) { }

        public void ExportDatabase(string content) { }
    }
}
