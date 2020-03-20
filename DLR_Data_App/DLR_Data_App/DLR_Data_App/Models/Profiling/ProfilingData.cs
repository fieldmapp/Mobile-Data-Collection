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
        /// <summary>
        /// Shared JsonSerializer. Is threadsafe, see https://www.newtonsoft.com/json/help/html/JsonNetVsDotNetSerializers.htm
        /// </summary>
        static JsonSerializer JsonSerializer;

        static ProfilingData()
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

        public string ProfilingId { get; set; }

        [JsonIgnore]
        public string ProfilingMenuItemsJson
        {
            get
            {
                using (var stringWriter = new StringWriter())
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    JsonSerializer.Serialize(jsonWriter, ProfilingMenuItems);
                    return stringWriter.ToString();
                }
            }
            set
            {
                using (var stringReader = new StringReader(value))
                using (var jsonReader = new JsonTextReader(stringReader))
                    ProfilingMenuItems = JsonSerializer.Deserialize<List<ProfilingMenuItem>>(jsonReader);
            }
        }

        [SQLite.Ignore]
        public List<ProfilingMenuItem> ProfilingMenuItems { get; set; }
    }
}
