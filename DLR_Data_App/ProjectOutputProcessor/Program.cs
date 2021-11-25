using CommandLine;
using ExifLib;
using FileHelpers;
using FileHelpers.Options;
using MoreLinq;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldCartographerProcessor
{
    public class Options
    {
        [Option('v', "verbose", Default = false, Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('i', "interactions", Default = "interactions.csv", Required = false, HelpText = "Set interaction csv file path.")]
        public string InteractionsPath { get; set; }

        [Option('p', "pos", Default = "complete.pos", Required = false, HelpText = "Set interaction csv file path.")]
        public string PositionPath { get; set; }

        [Option('l', "lanes", Default = "lanes", Required = false, HelpText = "Set lane shapefile path. (without extension, all relevant shapefile-files are needed)")]
        public string LanesPath { get; set; }

        [Option('o', "output", Default = "output.txt", Required = false, HelpText = "Set output json file path.")]
        public string OutputPath { get; set; }

        [Option('t', "temp-file-name", Default = "current", Required = false, HelpText = "Set path to use for the temp file.")]
        public string TempFilePath { get; set; }

        [Option('d', "date-format", Default = "yyyyMMdd_HHmmss", Required = false, HelpText = "Set format for dates in image names. See C# DateTime.ToString docs for more info.")]
        public string DateFormat { get; set; }
    }
    class Program
    {

        static async Task Main(string[] args)
        {
            bool argumentParsedFailed = false;
            Options options = null;

            void VerboseWriteLine(string s)
            {
                if (options.Verbose)
                    Console.WriteLine(s);
            }

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => options = o)
                .WithNotParsed(o => argumentParsedFailed = true);

            if (argumentParsedFailed)
                return;

            if (options == null)
                throw new ArgumentException("arguments given are malformed");

            if (!File.Exists(options.InteractionsPath))
                throw new ArgumentException($"Path {'"'}{options.InteractionsPath}{'"'} does not exist");

            if (!File.Exists(options.PositionPath))
                throw new ArgumentException($"Path {'"'}{options.PositionPath}{'"'} does not exist");

            VerboseWriteLine($"Trying to read {options.InteractionsPath}.");
            List<InteractionInfo> interactions = readInteractions(options);
            VerboseWriteLine($"Read successful.");

            VerboseWriteLine($"Trying to read {options.PositionPath}.");
            PosFileEntry[] posLog = readPosFile(options);
            VerboseWriteLine($"Read successful.");

            VerboseWriteLine($"Trying to read {options.LanesPath}.");
            List<(Geometry Geometry, string Name)> laneGeometries = readShapeFiles(options);
            VerboseWriteLine($"Read successful.");

            foreach (var p in interactions.Skip(13))
            {
                var time = p.UtcDateTime;
                PosFileEntry posEntry = posLog.MinBy(p => Math.Abs((p.DateTime - time).Ticks)).First();
                var timeOffset = posEntry.DateTime - time;
                var lat = posEntry.Latitude;
                var lon = posEntry.Longitude;
                
                var inside = laneGeometries[0].Geometry.Contains(new Point(lon, lat));
                Console.WriteLine($"{inside} with position age {timeOffset}");
            }
            bool c = laneGeometries[0].Geometry.Contains(new Point(12.23382887, 53.83023661));
        }

        private static PosFileEntry[] readPosFile(Options options)
        {
            var posFileReader = new FileHelperEngine<PosFileEntry>();
            var posLog = posFileReader.ReadFile(options.PositionPath);
            return posLog;
        }

        private static List<InteractionInfo> readInteractions(Options options)
        {

            var interactionsFileReader = new FileHelperEngine<InteractionInfo>();
            var interactionsLog = interactionsFileReader.ReadFile(options.InteractionsPath);
            return interactionsLog.ToList();
        }

        private static List<(Geometry Geometry, string Name)> readShapeFiles(Options options)
        {
            List<(Geometry Geometry, string Name)> laneGeometries = new List<(Geometry Geometry, string Name)>();
            var shapeFileReader = NetTopologySuite.IO.Shapefile.CreateDataReader(options.LanesPath, GeometryFactory.Floating);
            var shapeFileHeader = shapeFileReader.ShapeHeader;
            var dbaseHeader = shapeFileReader.DbaseHeader;
            int nameIndex = dbaseHeader.Fields.ToList().FindIndex(d => d.Name.Equals("name", StringComparison.InvariantCultureIgnoreCase)) + 1; // +1 because objects shapeFileReader.GetValues contains geometry as first entry

            while (shapeFileReader.Read())
            {
                Geometry geom = shapeFileReader.Geometry;

                object[] values = new object[shapeFileReader.FieldCount];
                string name = (string)shapeFileReader.GetValue(nameIndex);
                laneGeometries.Add((geom, name));
            }

            return laneGeometries;
        }
    }
}
