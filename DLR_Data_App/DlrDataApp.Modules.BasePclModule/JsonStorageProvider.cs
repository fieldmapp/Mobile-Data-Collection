using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DlrDataApp.Modules.SharedModule
{
    public class JsonStorageProvider
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

        //public void ExportAnswers(ProfilingResults results)
        //{
        //    if (!results.Results.Any())
        //        return;
        //
        //    var path = $"dlr_answers_{results.Results.First().UserId}.txt";
        //
        //    using (var storageStream = StorageAccessProvider.OpenFileWriteExternal(path))
        //    using (var streamWriter = new StreamWriter(storageStream))
        //    using (var jsonWriter = new JsonTextWriter(streamWriter))
        //        JsonSerializer.Serialize(jsonWriter, results);
        //}
        //
        //public void ExportDatabase(string content)
        //{
        //    var filename = "Fieldmapp_Database_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss", CultureInfo.InvariantCulture) + ".json";
        //
        //    using (var fileStream = StorageAccessProvider.OpenFileWriteExternal(filename))
        //    using (var streamWriter = new StreamWriter(fileStream))
        //    {
        //        streamWriter.Write(content);
        //    }
        //}
    }
}
