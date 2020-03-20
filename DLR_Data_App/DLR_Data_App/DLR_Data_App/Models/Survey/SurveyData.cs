using DLR_Data_App.Services;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DLR_Data_App.Models.Survey
{
    public class SurveyData
    {
        /// <summary>
        /// Shared JsonSerializer. Is threadsafe, see https://www.newtonsoft.com/json/help/html/JsonNetVsDotNetSerializers.htm
        /// </summary>
        static JsonSerializer JsonSerializer;

        static SurveyData()
        {
            JsonSerializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = IgnoreBindableObjectContractResolver.Instance,
                Formatting = Formatting.Indented,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
        }

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

        public string QuestionsJson { get; set; }

        public string ProfilingStagesJson { get; set; }

        public string SurveyId { get; set; }

        [JsonIgnore]
        public string SurveyMenuItemsJson
        {
            get
            {
                using (var stringWriter = new StringWriter())
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    JsonSerializer.Serialize(jsonWriter, SurveyMenuItems);
                    return stringWriter.ToString();
                }
            }
            set
            {
                using (var stringReader = new StringReader(value))
                using (var jsonReader = new JsonTextReader(stringReader))
                    SurveyMenuItems = JsonSerializer.Deserialize<List<SurveyMenuItem>>(jsonReader);
            }
        }

        [SQLite.Ignore]
        public List<SurveyMenuItem> SurveyMenuItems { get; set; }
    }
}
