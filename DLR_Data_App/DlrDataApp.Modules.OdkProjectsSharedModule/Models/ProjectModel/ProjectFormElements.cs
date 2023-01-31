using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel
{
    public class ProjectFormElements
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
        [ForeignKey(typeof(ProjectForm))]
        public int ProjectFormId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public string DefaultValue { get; set; }
        public bool ReadOnly { get; set; }
        public bool Required { get; set; }
        public string RequiredText { get; set; }
        public string Relevance { get; set; }
        public string Constraint { get; set; }
        public string InvalidText { get; set; }
        public string Calculate { get; set; }

        public string Options { get; set; }
        public bool Cascading { get; set; }
        public string Other { get; set; }
        public string Appearance { get; set; }
        public string Metadata { get; set; }
        public string Type { get; set; }
        public string Range { get; set; }
        public string Kind { get; set; }
        public string Length { get; set; }
    }
}
