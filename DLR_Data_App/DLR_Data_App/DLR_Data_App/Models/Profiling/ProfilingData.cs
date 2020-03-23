using DLR_Data_App.Services;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DLR_Data_App.Models.Profiling
{
    public class ProfilingData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Title of the profiling
        public string Title { get; set; }

        // Description of the profiling
        public string Description { get; set; }

        // Authors of the profiling
        public string Authors { get; set; }

        // Available languages
        public string Languages { get; set; }

        public string ProfilingId { get; set; }

        [JsonIgnore]
        public string TranslationsJson
        {
            get => JsonTranslator.GetJson(Translations);
            set => Translations = JsonTranslator.GetFromJson<Dictionary<string, string>>(value);
        }

        [SQLite.Ignore]
        public Dictionary<string, string> Translations { get; set; }

        [JsonIgnore]
        public string QuestionsJson
        {
            get => JsonTranslator.GetJson(Questions);
            set => Questions = JsonTranslator.GetFromJson<Dictionary<string, List<IQuestionContent>>>(value);
        }

        [SQLite.Ignore]
        public Dictionary<string, List<IQuestionContent>> Questions { get; set; }

        [JsonIgnore]
        public string ProfilingMenuItemsJson
        {
            get => JsonTranslator.GetJson(ProfilingMenuItems);
            set => ProfilingMenuItems = JsonTranslator.GetFromJson<List<ProfilingMenuItem>>(value);
        }

        [SQLite.Ignore]
        public List<ProfilingMenuItem> ProfilingMenuItems { get; set; }
    }
}
