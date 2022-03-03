using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FieldCartographerProcessor
{
    [DelimitedRecord(";")]
    class AreaInfo
    {
        public string LaneIdentifier { get; set; }
        public int ZoneIndex { get; set; }
        [FieldConverter(ConverterKind.Double, ",")]
        public double DistanceStartToZoneEntry { get; set; }
        [FieldConverter(ConverterKind.Double, ",")]
        public double StartLat { get; set; }
        [FieldConverter(ConverterKind.Double, ",")]
        public double StartLong { get; set; }
        [FieldConverter(ConverterKind.Double, ",")]
        public double DistanceEndToZoneEntry { get; set; }
        [FieldConverter(ConverterKind.Double, ",")]
        public double EndLat { get; set; }
        [FieldConverter(ConverterKind.Double, ",")]
        public double EndLong { get; set; }
        
        public string EstYieldReduction { get; set; }
        public string Cause { get; set; }
        public string UserName { get; set; }
    }
}
