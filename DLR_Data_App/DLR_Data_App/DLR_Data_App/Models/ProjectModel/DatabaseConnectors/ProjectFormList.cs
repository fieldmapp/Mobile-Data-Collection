using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace DLR_Data_App.Models.ProjectModel.DatabaseConnectors
{
  /**
   * List the id of the project and the corresponding form
   */
  class ProjectFormList
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int FormId { get; set; }
  }
}
