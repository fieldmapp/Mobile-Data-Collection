using System;
using System.IO;
using DLR_Data_App.Services;

namespace com.DLR.DLR_Data_App.Droid
{
  /**
   * Storing the export files on device as json
   */
  public class FileManager : IFileManager
  {
    /**
     * Implementation for storing files under android
     */
    public bool WriteExportFile(string content)
    {
      var filename = "Fieldmapp_" + DateTime.UtcNow + ".json";
      filename = filename.Replace(' ', '_');
      filename = filename.Replace(':', '_');
      var storageFolder =
        Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
      var path = Path.Combine(storageFolder, filename);

      // check if device is writable
      if (Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState))
      {
        //File.WriteAllText(path, content);
        //return true;
      }

      return false;
    }
  }
}