using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel
{
    class ActiveProjectInfo
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [ForeignKey(typeof(Project))]
        public int ActiveProjectId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public Project ActiveProject { get; set; }
    }
}
