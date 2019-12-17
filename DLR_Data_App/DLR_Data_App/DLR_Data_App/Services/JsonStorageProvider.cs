﻿using DLR_Data_App.Models.Survey;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DLR_Data_App.Services
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

        public SurveyResults LoadAnswers(string userId)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "answers_" + userId);

            using (var storageStream = StorageAccessProvider.OpenFileRead(path))
            {
                if (storageStream.Length == 0)
                    return new SurveyResults() { UserId = userId, Results = new List<SurveyResult>() };
                using (var streamReader = new StreamReader(storageStream))
                using (var jsonReader = new JsonTextReader(streamReader))
                    return JsonSerializer.Deserialize<SurveyResults>(jsonReader);
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

        public SurveyData LoadSurveyData()
        {
            return DeserializeFromAsset<SurveyData>("surveys");
        }

        public void SaveAnswers(SurveyResults results)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "answers_" + results.UserId);

            using (var storageStream = StorageAccessProvider.OpenFileWrite(path))
            using (var streamWriter = new StreamWriter(storageStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
                JsonSerializer.Serialize(jsonWriter, results);
        }

        public void ExportAnswers(SurveyResults results)
        {
            var path = $"dlr_answers_{results.UserId}.txt";

            using (var storageStream = StorageAccessProvider.OpenFileWriteExternal(path))
            using (var streamWriter = new StreamWriter(storageStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
                JsonSerializer.Serialize(jsonWriter, results);
        }

        public void ExportDatabase(string content)
        {
            var filename = "Fieldmapp_Database_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmf") + ".json";

            using (var fileStream = StorageAccessProvider.OpenFileWriteExternal(filename))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(content);
            }
        }

        public Dictionary<string, string> LoadTranslations()
        {
            return DeserializeFromAsset<Dictionary<string, string>>("translations");
        }
    }
}
