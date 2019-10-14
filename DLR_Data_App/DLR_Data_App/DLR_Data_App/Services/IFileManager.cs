namespace DLR_Data_App.Services
{
  /**
  * Interface for platform specific implementation of file export
  */
  public interface IFileManager
  {
    /**
     * Exporting content of database to JSON file
     * @param content Text that should be exported
     * @returns Status of success
     */
    bool WriteExportFile(string content);
  }
}
