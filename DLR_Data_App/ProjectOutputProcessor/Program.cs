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

            if (!File.Exists(options.PositionPath))
                throw new ArgumentException($"Path {'"'}{options.PositionPath}{'"'} does not exist");

            VerboseWriteLine($"Trying to read {options.PositionPath}.");
            PosFileEntry[] posLog = readPosFile(options);
            VerboseWriteLine($"Read successful.");

            VerboseWriteLine($"Trying to read {options.LanesPath}.");
            List<LaneInfo> laneInfos = readShapeFiles(options);

            VerboseWriteLine($"Read successful.");

            const int zoneCount = 7;
            var zonesHistory = new List<AreaInfo>();

            foreach (var file in Directory.GetFiles("interactions"))
            {
                var interactions = readInteractions(file);
                var username = file.Split('_')[1];
                ProcessInteractions(interactions, posLog, laneInfos, zoneCount, username, zonesHistory);
            }

            zonesHistory.Sort((a, b) => a.StartInputDateTime.CompareTo(b.StartInputDateTime));
            var fileWriter = new FileHelperEngine<AreaInfo>();
            fileWriter.HeaderText = fileWriter.GetFileHeader();
            fileWriter.WriteFile("out.csv", zonesHistory);
        }

        private static void ProcessInteractions(List<InteractionInfo> interactions, PosFileEntry[] posLog, List<LaneInfo> lanes, int zoneCount, string username, List<AreaInfo> zonesHistory)
        {
            var activeZones = new AreaInfo[zoneCount];

            void finalizeZone(int zoneIndex)
            {
                if (activeZones[zoneIndex] == null)
                    return;
                zonesHistory.Add(activeZones[zoneIndex]);
                activeZones[zoneIndex] = null;
            }

            LaneInfo currentLane = null;

            foreach (var interaction in interactions.Where(i => !i.Action.StartsWith("Miss")))
            {
                var time = interaction.UtcDateTime;
                PosFileEntry posEntry = posLog.MinBy(p => Math.Abs((p.DateTime - time).Ticks)).First();
                var timeOffset = posEntry.DateTime - time;

                var pos = new Point(posEntry.Longitude, posEntry.Latitude);

                var newLane = lanes.FirstOrDefault(l => l.Geometry.Contains(pos));
                if (newLane != null)
                {
                    if (newLane != currentLane)
                    {
                        Console.WriteLine($"Entered lane {newLane.Name} at {time} at position {pos}");
                        // close all zones
                        for (int i = 0; i < zoneCount; i++)
                        {
                            if (activeZones[i] != null)
                            {
                                activeZones[i].DistanceEndToZoneEntry = Helpers.DistanceBetween(currentLane.ExitPoint, currentLane.EntryPoint);
                                activeZones[i].EndLong = Helpers.DistanceBetween(currentLane.ExitPoint, currentLane.EntryPoint);
                                activeZones[i].EndLat = Helpers.DistanceBetween(currentLane.ExitPoint, currentLane.EntryPoint);
                                finalizeZone(i);
                            }
                        }
                        currentLane = newLane;
                    }
                }

                if (currentLane == null)
                    continue;

                if (interaction.Action == "open")
                {
                    finalizeZone(interaction.LaneIndex);
                    activeZones[interaction.LaneIndex] = new AreaInfo
                    {
                        ZoneIndex = interaction.LaneIndex,
                        UserName = username,
                        DistanceStartToZoneEntry = Helpers.DistanceBetween(pos, currentLane.EntryPoint),
                        StartLong = pos.X,
                        StartLat = pos.Y,
                        StartInputDateTime = time,
                        LaneIdentifier = currentLane.Name
                    };
                }
                else if (interaction.Action == "close" && activeZones[interaction.LaneIndex] != null)
                {
                    activeZones[interaction.LaneIndex].DistanceEndToZoneEntry = Helpers.DistanceBetween(pos, currentLane.EntryPoint);
                    activeZones[interaction.LaneIndex].EndLong = pos.X;
                    activeZones[interaction.LaneIndex].EndLat = pos.Y; ;
                    activeZones[interaction.LaneIndex].EndInputDateTime = time;
                }
                else if (interaction.Action == "canceled")
                {
                    for (int i = 0; i < zoneCount; i++)
                    {
                        if (activeZones[i] == null)
                            continue;
                        else if (activeZones[i].EndInputDateTime == default)
                            finalizeZone(i);
                        else
                        activeZones[i] = null;
                        
                    }
                }
                else if (interaction.Action.StartsWith("cause=") && activeZones[interaction.LaneIndex] != null)
                {
                    activeZones[interaction.LaneIndex].Cause = interaction.Action["cause=".Length..];
                    activeZones[interaction.LaneIndex].CauseInputLong = pos.X;
                    activeZones[interaction.LaneIndex].CauseInputLat = pos.Y;
                    activeZones[interaction.LaneIndex].CauseInputDateTime = time;
                }
                else if (interaction.Action.StartsWith("damage=") && activeZones[interaction.LaneIndex] != null)
                {
                    activeZones[interaction.LaneIndex].EstYieldReduction = interaction.Action["damage=".Length..];
                    activeZones[interaction.LaneIndex].EstYieldReductionInputLong = pos.X;
                    activeZones[interaction.LaneIndex].EstYieldReductionInputLat = pos.Y;
                    activeZones[interaction.LaneIndex].EstYieldReductionInputDateTime = time;
                }
            }

            for (int i = 0; i < zoneCount; i++)
            {
                finalizeZone(i);
            }
        }

        private static PosFileEntry[] readPosFile(Options options)
        {
            var posFileReader = new FileHelperEngine<PosFileEntry>();
            var posLog = posFileReader.ReadFile(options.PositionPath);
            return posLog;
        }

        private static List<InteractionInfo> readInteractions(string interactionsPath)
        {

            var interactionsFileReader = new FileHelperEngine<InteractionInfo>();
            var interactionsLog = interactionsFileReader.ReadFile(interactionsPath);
            return interactionsLog.ToList();
        }

        private static List<LaneInfo> readShapeFiles(Options options)
        {
            var lanes = new List<LaneInfo>();
            var shapeFileReader = NetTopologySuite.IO.Shapefile.CreateDataReader(options.LanesPath, GeometryFactory.Floating);
            var shapeFileHeader = shapeFileReader.ShapeHeader;
            var dbaseHeader = shapeFileReader.DbaseHeader;
            List<NetTopologySuite.IO.DbaseFieldDescriptor> headerList = dbaseHeader.Fields.ToList();
            int nameIndex = headerList.FindIndex(d => d.Name.Equals("name", StringComparison.InvariantCultureIgnoreCase)) + 1; // +1 because objects shapeFileReader.GetValues contains geometry as first entry
            int entryLatIndex = headerList.FindIndex(d => d.Name.Equals("EntryLat", StringComparison.InvariantCultureIgnoreCase)) + 1; // +1 because objects shapeFileReader.GetValues contains geometry as first entry
            int entryLongIndex = headerList.FindIndex(d => d.Name.Equals("EntryLong", StringComparison.InvariantCultureIgnoreCase)) + 1; // +1 because objects shapeFileReader.GetValues contains geometry as first entry
            int exitLatIndex = headerList.FindIndex(d => d.Name.Equals("ExitLat", StringComparison.InvariantCultureIgnoreCase)) + 1; // +1 because objects shapeFileReader.GetValues contains geometry as first entry
            int exitLongIndex = headerList.FindIndex(d => d.Name.Equals("ExitLong", StringComparison.InvariantCultureIgnoreCase)) + 1; // +1 because objects shapeFileReader.GetValues contains geometry as first entry


            while (shapeFileReader.Read())
            {
                Geometry geom = shapeFileReader.Geometry;

                object[] values = new object[shapeFileReader.FieldCount];
                string name = (string)shapeFileReader.GetValue(nameIndex);
                if (name == null)
                    throw new Exception("The lanes name must not be null.");
                var entryPoint = new Point((double)shapeFileReader.GetValue(entryLongIndex), (double)shapeFileReader.GetValue(entryLatIndex));
                var exitPoint = new Point((double)shapeFileReader.GetValue(exitLongIndex), (double)shapeFileReader.GetValue(exitLatIndex));
                lanes.Add(new LaneInfo(geom, entryPoint, exitPoint, name));
            }

            if (lanes.Select(l => l.Name).ContainsDuplicates())
                throw new Exception("The lanes shapefile contains multiple features with the same name.");

            foreach (var geometry1 in lanes.Select(g => g.Geometry))
            {
                foreach (var geometry2 in lanes.Select(g => g.Geometry).Where(g => g != geometry1))
                {
                    if (geometry1.Intersects(geometry2))
                        throw new Exception("There are intersecting lanes defined in the lanes shapefile.");
                }
            }

            return lanes;
        }
    }
}
