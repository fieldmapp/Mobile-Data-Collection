using SQLite;

namespace DLR_Data_App.Models.ProjectModel.DatabaseConnectors
{
    /// <summary>
    /// This model determines which user is using which project.
    /// </summary>
    public class ProjectUserConnection
    {
        // Table ID
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Project ID
        public int ProjectId { get; set; }

        // User ID
        public int UserId { get; set; }
    }
}
