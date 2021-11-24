using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FieldCartographerProcessor
{
    internal static class Helpers
    {
        public static DateTime FromGPST(this DateTime gpst)
        {

            // Table from https://confluence.qps.nl/qinsy/latest/en/utc-to-gps-time-correction-32245263.html
            List<DateTime> leapSecondDays = new List<DateTime>
            {
                new DateTime(1981, 7, 1),
                new DateTime(1982, 7, 1),
                new DateTime(1983, 7, 1),
                new DateTime(1985, 7, 1),
                new DateTime(1988, 1, 1),
                new DateTime(1990, 1, 1),
                new DateTime(1991, 1, 1),
                new DateTime(1992, 7, 1),
                new DateTime(1993, 7, 1),
                new DateTime(1994, 7, 1),
                new DateTime(1996, 1, 1),
                new DateTime(1997, 7, 1),
                new DateTime(1999, 1, 1),
                new DateTime(2006, 1, 1),
                new DateTime(2009, 1, 1),
                new DateTime(2012, 7, 1),
                new DateTime(2015, 7, 1),
                new DateTime(2017, 1, 1)
            };
            var leapSeconds = leapSecondDays.Count(leapSecondDate => gpst > leapSecondDate);
            return gpst + TimeSpan.FromSeconds(leapSeconds);
        }
    }
}
