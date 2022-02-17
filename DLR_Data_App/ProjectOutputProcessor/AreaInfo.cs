using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FieldCartographerProcessor
{
    public enum DamageType
    {
        Low,
        Medium,
        High
    }
    public enum DamageCause
    {
        SandLens,
        Compaction,
        Headland,
        Dome,
        Slope,
        ForrestEdge,
        DryStress,
        WatterLogging,
        MouseEating
    }


    [DelimitedRecord(";")]
    class AreaInfo
    {
        public int ZoneIndex { get; set; }
        [FieldConverter(ConverterKind.Double, ",")]
        public double DistanceStartToZoneEntry { get; set; }
        [FieldConverter(ConverterKind.Double, ",")]
        public double DistanceEndToZoneEntry { get; set; }
        public DamageType Type { get; set; }
        public DamageCause Cause { get; set; }
        public string UserName { get; set; }
    }
}
