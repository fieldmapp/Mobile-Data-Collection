using DlrDataApp.Modules.Base.Shared;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel
{
    class ActiveProjectInfo : IActiveElementInfo<Project>
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [ForeignKey(typeof(Project))]
        public int ActiveElementId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public Project ActiveElement { get; set; }
    }
}
