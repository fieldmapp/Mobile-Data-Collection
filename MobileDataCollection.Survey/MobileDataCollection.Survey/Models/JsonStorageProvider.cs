using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    public class JsonStorageProvider : IStorageProvider
    {
        IStorageAccessProvider StorageAccessProvider;
        JsonSerializer JsonSerializer;

        public JsonStorageProvider(IStorageAccessProvider provider)
        {
            StorageAccessProvider = provider;
            JsonSerializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = IgnoreBindableObjectContractResolver.Instance,
                Formatting = Formatting.Indented,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
        }

        public Dictionary<string, List<IUserAnswer>> LoadAnswers()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "answers");

            using (var storageStream = StorageAccessProvider.OpenFileRead(path))
            {
                if (storageStream.Length == 0)
                    return new Dictionary<string, List<IUserAnswer>>();
                using (var streamReader = new StreamReader(storageStream))
                using (var jsonReader = new JsonTextReader(streamReader))
                    return JsonSerializer.Deserialize<Dictionary<string, List<IUserAnswer>>>(jsonReader);
            }
        }

        private T DeserializeFromAsset<T>(string path)
        {
            using (var storageStream = StorageAccessProvider.OpenAsset(path))
            using (var streamReader = new StreamReader(storageStream))
            using (var jsonReader = new JsonTextReader(streamReader))
                return JsonSerializer.Deserialize<T>(jsonReader);
        }

        public Dictionary<string, List<IQuestionContent>> LoadQuestions()
        {
            return DeserializeFromAsset<Dictionary<string, List<IQuestionContent>>>("questions");
        }

        public List<SurveyMenuItem> LoadSurveyMenuItems()
        {
            return DeserializeFromAsset<List<SurveyMenuItem>>("surveys");
        }

        public void SaveAnswers(Dictionary<string, List<IUserAnswer>> answers)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "answers");

            using (var storageStream = StorageAccessProvider.OpenFileWrite(path))
            using (var streamWriter = new StreamWriter(storageStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
                JsonSerializer.Serialize(jsonWriter, answers);
        }

        //TODO: Remove Save and Load
        public void Save<T>(T savedata)
        {
            using (var storageStream = StorageAccessProvider.OpenFileWrite("answers"))
            using (var streamWriter = new StreamWriter(storageStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
                JsonSerializer.Serialize(jsonWriter, savedata);
        }

        public T Load<T>()
        {
            using (var storageStream = StorageAccessProvider.OpenFileRead("1.txt"))
            using (var streamReader = new StreamReader(storageStream))
            using (var jsonReader = new JsonTextReader(streamReader))
                return JsonSerializer.Deserialize<T>(jsonReader);
        }
    }
}
