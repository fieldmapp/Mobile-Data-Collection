using System.Collections.Generic;

using SQLite;

namespace DLR_Data_App.Models.ProjectModel
{
    /// <summary>
    /// Contains all information about a form
    /// </summary>
    public class ProjectForm
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }

        public int ProjectId { get; set; }

        public string SurveyId { get; set; }

        [Ignore]
        public List<ProjectFormElements> ElementList { get; set; }

        [Ignore]
        public ProjectFormMetadata Metadata { get; set; }
    }
}
