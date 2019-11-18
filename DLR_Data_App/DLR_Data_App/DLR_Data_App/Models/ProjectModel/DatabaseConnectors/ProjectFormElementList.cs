using SQLite;

namespace DLR_Data_App.Models.ProjectModel.DatabaseConnectors
{
    /// <summary>
    /// Class containing form id, element id, id and metadata id
    /// </summary>
    public class ProjectFormElementList
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int FormId { get; set; }
        public int ElementId { get; set; }
        public int MetadataId { get; set; }
    }
}
