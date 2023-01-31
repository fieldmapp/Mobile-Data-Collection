using DlrDataApp.Modules.Base.Shared;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.Profiling.Shared.Models
{
    public class ActiveProfilingInfo : IActiveElementInfo<ProfilingData>
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [ForeignKey(typeof(ProfilingData))]
        public int ActiveElementId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public ProfilingData ActiveElement { get; set; }
    }
}
