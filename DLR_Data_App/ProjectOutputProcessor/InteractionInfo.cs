using FileHelpers;
using FileHelpers.Events;
using System;

namespace FieldCartographerProcessor
{
    [DelimitedRecord(","), IgnoreFirst]
    public class InteractionInfo : INotifyRead
    {
        /// <summary>
        /// Default constructor. Needed for CsvHelper
        /// </summary>
        public InteractionInfo()
        {

        }

        [FieldConverter(ConverterKind.Date, "MM/dd/yyyy HH:mm:ss", "")] // empty string as culture -> invariant culture is being used
        public DateTime UtcDateTime { get; set; }
        public int LaneIndex { get; set; }
        [FieldQuoted('"')]
        public string Action { get; set; }

        public void AfterRead(AfterReadEventArgs e)
        {
            UtcDateTime = new DateTime(UtcDateTime.Ticks, DateTimeKind.Utc);
        }

        public void BeforeRead(BeforeReadEventArgs e)
        {

        }
    }
}
