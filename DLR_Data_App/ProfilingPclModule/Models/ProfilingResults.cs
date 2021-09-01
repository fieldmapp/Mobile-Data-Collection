using DlrDataApp.Modules.Base.Shared;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace DlrDataApp.Modules.Profiling.Shared.Models
{
    public class ProfilingResults
    {
        public List<ProfilingResult> Results { get; set; } = new List<ProfilingResult>();
    }

    public class ProfilingResult
    {
        [PrimaryKey, AutoIncrement]
        public int? ProfilingResultId { get; set; }

        public DateTime TimeStamp { get; set; }

        [JsonIgnore]
        public string UserAnswersJson { get; set; }

        [TextBlob(nameof(UserAnswersJson))]
        public Dictionary<string, List<IUserAnswer>> UserAnswers { get; set; }

        public int UserId { get; set; }

        public string ProfilingId { get; set; }
    }
}
