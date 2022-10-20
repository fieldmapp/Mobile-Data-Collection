using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel
{
    /// <summary>
    /// Model of a project containing all general informations about it.
    /// </summary>
    public class Project
    {

        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        // Title of the project
        public string Title { get; set; }

        // Description of the project
        public string Description { get; set; }

        // Authors of the project
        public string Authors { get; set; }

        // Available languages
        public string Languages { get; set; }

        public string ProfilingId { get; set; }

        public string PreviewPattern { get; set; }

        // List of forms
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ProjectForm> FormList { get; set; } = new List<ProjectForm>();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ProjectResult> Results { get; set; } = new List<ProjectResult>();

    }
}
