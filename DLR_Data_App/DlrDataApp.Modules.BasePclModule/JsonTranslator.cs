using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DlrDataApp.Modules.SharedModule
{
    public static class JsonTranslator
    {
        /// <summary>
        /// Shared JsonSerializer. Is threadsafe, see https://www.newtonsoft.com/json/help/html/JsonNetVsDotNetSerializers.htm
        /// </summary>
        public static JsonSerializer JsonSerializer { get; }

        static JsonTranslator()
        {
            JsonSerializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = IgnoreBindableObjectContractResolver.Instance,
                Formatting = Formatting.Indented,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
        }

        public static string GetJson(object input)
        {
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                JsonSerializer.Serialize(jsonWriter, input);
                return stringWriter.ToString();
            }
        }

        public static T GetFromJson<T>(string json) where T:new()
        {
            if (json == null)
                return new T();

            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
                return JsonSerializer.Deserialize<T>(jsonReader);
        }
    }
}
