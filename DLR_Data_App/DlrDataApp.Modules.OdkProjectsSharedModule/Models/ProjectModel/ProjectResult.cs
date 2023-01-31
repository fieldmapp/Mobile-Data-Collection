using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel
{
    public class ProjectResult
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [ForeignKey(typeof(Project))]
        public int ProjectId { get; set; }

        [JsonIgnore]
        public string DataJson { get; set; }

        [TextBlob(nameof(Data))]
        public Dictionary<string, string> Data { get; set; }
    }
}
