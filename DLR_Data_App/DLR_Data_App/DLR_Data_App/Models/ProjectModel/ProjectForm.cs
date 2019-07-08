using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace DLR_Data_App.Models.ProjectModel
{
  public class ProjectForm
  {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; }

    [Ignore]
    public List<ProjectFormElements> ElementList { get; set; }

    [Ignore]
    public ProjectFormMetadata Metadata { get; set; }
  }
}
