using SQLite;

namespace DLR_Data_App.Models.ProjectModel.DatabaseConnectors
{
  /**
   * List the id of the form and the corresponding element
   */
  public class ProjectFormElementList
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int FormId { get; set; }
    public int ElementId { get; set; }
    public int MetadataId { get; set; }
  }
}
