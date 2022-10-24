using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FieldCartographerProcessor
{
    [DelimitedRecord(";")]
    class AreaInfo
    {
        const string DecimalSeperator = ",";
        const string DateFormat = "dd/MM/yyyy HH:mm:ss:f";
        public string LaneIdentifier { get; set; }

        public int ZoneIndex { get; set; }

        
        [FieldConverter(ConverterKind.Double, DecimalSeperator)]
        public double DistanceStartToZoneEntry { get; set; }

        [FieldConverter(ConverterKind.Double, DecimalSeperator)]
        public double StartLat { get; set; }

        [FieldConverter(ConverterKind.Double, DecimalSeperator)]
        public double StartLong { get; set; }

        [FieldConverter(ConverterKind.Date, DateFormat)]
        public DateTime StartInputDateTime { get; set; }


        [FieldConverter(ConverterKind.Double, DecimalSeperator)]
        public double DistanceEndToZoneEntry { get; set; }

        [FieldConverter(ConverterKind.Double, DecimalSeperator)]
        public double EndLat { get; set; }

        [FieldConverter(ConverterKind.Double, DecimalSeperator)]
        public double EndLong { get; set; }

        [FieldConverter(ConverterKind.Date, DateFormat)]
        public DateTime EndInputDateTime { get; set; }


        public string EstYieldReduction { get; set; }

        [FieldConverter(ConverterKind.Double, DecimalSeperator)]
        public double EstYieldReductionInputLat { get; set; }

        [FieldConverter(ConverterKind.Double, DecimalSeperator)]
        public double EstYieldReductionInputLong { get; set; }

        [FieldConverter(ConverterKind.Date, DateFormat)]
        public DateTime EstYieldReductionInputDateTime { get; set; }


        public string Cause { get; set; }

        [FieldConverter(ConverterKind.Double, DecimalSeperator)]
        public double CauseInputLat { get; set; }

        [FieldConverter(ConverterKind.Double, DecimalSeperator)]
        public double CauseInputLong { get; set; }

        [FieldConverter(ConverterKind.Date, DateFormat)]
        public DateTime CauseInputDateTime { get; set; }

        public string UserName { get; set; }
    }
}
