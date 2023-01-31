using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel
{
    public class ProjectFormMetadata
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        [ForeignKey(typeof(ProjectForm))]
        public int ProjectFormId { get; set; }
        public int Version { get; set; }
        public string Languages { get; set; }
        public string OptionsPresets { get; set; }
        public string Htitle { get; set; }
        public string InstanceName { get; set; }
        public string PublicKey { get; set; }
        public string SubmissionUrl { get; set; }
    }
}
