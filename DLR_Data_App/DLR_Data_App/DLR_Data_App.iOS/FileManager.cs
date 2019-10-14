using System;
using System.IO;
using DLR_Data_App.Services;

namespace DLR_Data_App.iOS
{
  public class FileManager : IFileManager
  {
    public bool WriteExportFile(string content)
    {
      var filename = "DLR_Fieldmapp_" + DateTime.UtcNow + ".json";
      var localpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      var storageFolder = Path.Combine(localpath, filename);

      File.WriteAllText(storageFolder, content);

      return false;
    }
  }
}