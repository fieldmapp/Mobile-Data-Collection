using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models.Profiling
{
    public class ProfilingResults
    {
        public string UserId { get; set; }
        public List<ProfilingResult> Results { get; set; } = new List<ProfilingResult>();
        public int ProjectsFilledSinceLastProfilingCompletion = 0;
    }

    public class ProfilingResult
    {
        public DateTime TimeStamp { get; set; }
        public Dictionary<string, List<IUserAnswer>> UserAnswers { get; set; } = new Dictionary<string, List<IUserAnswer>>();
    }
}
