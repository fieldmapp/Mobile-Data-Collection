using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.Profiling.Shared.Models
{
    public class ActiveProfilingInfo
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [ForeignKey(typeof(ProfilingData))]
        public int ActiveProfilingId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public ProfilingData ActiveProfiling { get; set; }
    }
}
