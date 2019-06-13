using SQLite;

using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models
{
  /*
   * Model of a project containing all general informations about it
   */
  class Project
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Title of the project
    public string Title { get; set; }

    // Description of the project
    public string Description { get; set; }

    // Authors of the project
    public string Authors { get; set; }

    // List of forms
    public List<ProjectForm> Formlist { get; set; }
  }
}
