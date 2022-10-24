using FileHelpers;
using FileHelpers.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace FieldCartographerProcessor
{
    [FixedLengthRecord()]
    class PosFileEntry : INotifyRead
    {
        [FieldConverter(ConverterKind.Date, "yyyy/MM/dd HH:mm:ss.fff")]
        [FieldFixedLength(23)]
        public DateTime DateTime { get; set; }
        
        [FieldFixedLength(15)]
        public double Latitude { get; set; }

        [FieldFixedLength(15)]
        public double Longitude { get; set; }
        
        [FieldFixedLength(11)]
        public double Height { get; set; }

        [FieldFixedLength(4)]
        public int Quality { get; set; }
        
        [FieldFixedLength(4)]
        public int SatelliteCount { get; set; }
        
        [FieldFixedLength(9)]
        public double SDN { get; set; }


        [FieldFixedLength(9)]
        public double SDE { get; set; }

        [FieldFixedLength(9)]
        public double SDU { get; set; }

        [FieldFixedLength(9)]
        public double SDNE { get; set; }

        [FieldFixedLength(9)]
        public double SDEU { get; set; }

        [FieldFixedLength(9)]
        public double SDUN { get; set; }

        [FieldFixedLength(7)]
        public double Age { get; set; }

        [FieldFixedLength(7)]
        public double Ratio { get; set; }

        public void AfterRead(AfterReadEventArgs e)
        {
            DateTime = DateTime.FromGPST();
        }

        public void BeforeRead(BeforeReadEventArgs e)
        {
            if (e.RecordLine.StartsWith("%"))
                e.SkipThisRecord = true;
        }
    }
}
