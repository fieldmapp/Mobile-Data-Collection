using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FieldCartographerProcessor
{
    public class InteractionInfo
    {
        /// <summary>
        /// Default constructor. Needed for CsvHelper
        /// </summary>
        public InteractionInfo()
        {

        }
        public InteractionInfo(DateTime utcDateTime, int laneIndex, string action)
        {
            UtcDateTime = utcDateTime;
            LaneIndex = laneIndex;
            Action = action;
        }
        [Index(0)]
        public DateTime UtcDateTime { get; set; }
        [Index(1)]
        public int LaneIndex { get; set; }
        [Index(2)]
        public string Action { get; set; }
    }
}
