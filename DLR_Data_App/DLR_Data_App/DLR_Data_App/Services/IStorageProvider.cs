using DLR_Data_App.Models.Profiling;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Services
{
    public interface IStorageProvider
    {
        void ExportAnswers(ProfilingResults results);
        void ExportDatabase(string content);
    }
}
