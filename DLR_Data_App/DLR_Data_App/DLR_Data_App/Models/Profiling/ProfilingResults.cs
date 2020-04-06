using DLR_Data_App.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models.Profiling
{
    public class ProfilingResults
    {
        public List<ProfilingResult> Results { get; set; } = new List<ProfilingResult>();
    }

    public class ProfilingResult
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int ProfilingResultId { get; set; } = -1;

        public DateTime TimeStamp { get; set; }

        [JsonIgnore]
        public string UserAnswersJson
        {
            get => JsonTranslator.GetJson(UserAnswers);
            set => UserAnswers = JsonTranslator.GetFromJson<Dictionary<string, List<IUserAnswer>>>(value);
        }

        [SQLite.Ignore]
        public Dictionary<string, List<IUserAnswer>> UserAnswers { get; set; }

        public int UserId { get; set; }

        public string ProfilingId { get; set; }
    }
}
