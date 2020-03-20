using SQLite;
using System.Collections.Generic;

namespace DLR_Data_App.Models.ProjectModel
{
    /// <summary>
    /// Model of a project containing all general informations about it.
    /// </summary>
    public class Project
    {
        public Project()
        {
            FormList = new List<ProjectForm>();
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Title of the project
        public string Title { get; set; }

        // Description of the project
        public string Description { get; set; }

        // Authors of the project
        public string Authors { get; set; }

        // Available languages
        public string Languages { get; set; }

        // select project as current project
        public bool CurrentProject { get; set; }

        public string ProfilingId { get; set; }

        // List of forms
        [Ignore]
        public List<ProjectForm> FormList { get; set; }
    }
}
