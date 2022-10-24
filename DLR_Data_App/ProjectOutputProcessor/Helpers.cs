using NetTopologySuite.Geometries;
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
            // Requires updates when UTC leap seconds are announced
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
            return new DateTime(gpst.Ticks, DateTimeKind.Utc) - TimeSpan.FromSeconds(leapSeconds);
        }

        public static bool ContainsDuplicates<T>(this IEnumerable<T> seq)
        {
            return seq.GroupBy(x => x).Any(x => x.Count() > 1);
        }

        public static double DistanceBetween(Point point1, Point point2)
        {
            // haversine from https://stackoverflow.com/a/41623738/8512719
            const double r = 6371; // meters
            
            var sdlat = Math.Sin((point2.Y - point1.Y) / 2);
            var sdlon = Math.Sin((point2.X - point1.X) / 2);
            var q = sdlat * sdlat + Math.Cos(point1.Y) * Math.Cos(point2.Y) * sdlon * sdlon;
            var d = 2 * r * Math.Asin(Math.Sqrt(q));
            
            return d;
        }
    }
}
