using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectModel
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
        public List<ProjectForm> FormList { get; set; }
    }
}
