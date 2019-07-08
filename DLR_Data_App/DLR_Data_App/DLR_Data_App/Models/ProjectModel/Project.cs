using SQLite;

using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models.ProjectModel
{
  /*
   * Model of a project containing all general informations about it
   */
  public class Project
  {
    public Project()
    {
      Formlist = new List<ProjectForm>();
    }

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Title of the project
    public string Title { get; set; }

    // Description of the project
    public string Description { get; set; }

    // Authors of the project
    public string Authors { get; set; }

    // Secret password of project, if not set or null there is no password
    // Stored in SHA512
    public string Secret { get; set; }

    // Available languages
    public string Languages { get; set; }

    // select project as current project
    public bool CurrentProject { get; set; }

    // List of forms
    [Ignore]
    public List<ProjectForm> Formlist { get; set; }
  }
}
