using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models.ProjectModel.DatabaseConnectors
{
  /**
   * This model determines which user is using which project
   * It also stores the result of the profiling test
   */
  public class ProjectUserConnection
  {
    // Table ID
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Project ID
    public int ProjectId { get; set; }

    // User ID
    public int UserId { get; set; }

    // Result of the profiling test
    public double Weight { get; set; }
  }
}
