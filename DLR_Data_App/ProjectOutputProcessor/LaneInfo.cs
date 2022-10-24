using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace FieldCartographerProcessor
{
    class LaneInfo
    {
        public LaneInfo(Geometry geometry, Point entryPoint, Point exitPoint, string name)
        {
            EntryPoint = entryPoint;
            ExitPoint = exitPoint;
            Geometry = geometry;
            Name = name;
        }

        public Geometry Geometry { get; set; }
        public Point EntryPoint { get; set; }
        public Point ExitPoint { get; set; }
        public string Name { get; set; }
    }
}
