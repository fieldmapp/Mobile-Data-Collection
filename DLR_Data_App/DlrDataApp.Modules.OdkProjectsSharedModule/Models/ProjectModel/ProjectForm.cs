using System.Collections.Generic;

using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel
{
    /// <summary>
    /// Contains all information about a form
    /// </summary>
    public class ProjectForm
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        public string Title { get; set; }

        [ForeignKey(typeof(Project))]
        public int ProjectId { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ProjectFormElements> ElementList { get; set; } = new List<ProjectFormElements>();

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ProjectFormMetadata Metadata { get; set; } = new ProjectFormMetadata();
    }
}
