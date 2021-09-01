using DlrDataApp.Modules.Base.Shared;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DlrDataApp.Modules.Profiling.Shared.Models
{
    public class ProfilingData
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        // Title of the profiling
        public string Title { get; set; }

        // Description of the profiling
        public string Description { get; set; }

        // Authors of the profiling
        public string Authors { get; set; }
        public string ProfilingId { get; set; }

        [JsonIgnore]
        public string TranslationsJson { get; set; }

        [TextBlob(nameof(TranslationsJson))]
        public Dictionary<string, string> Translations { get; set; }

        [JsonIgnore]
        public string QuestionsJson { get; set; }

        [TextBlob(nameof(QuestionsJson))]
        public Dictionary<string, List<IQuestionContent>> Questions { get; set; }

        [JsonIgnore]
        public string ProfilingMenuItemsJson { get; set; }

        [TextBlob(nameof(ProfilingMenuItemsJson))]
        public List<ProfilingMenuItem> ProfilingMenuItems { get; set; }
    }
}
