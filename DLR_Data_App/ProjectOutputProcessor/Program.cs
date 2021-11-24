using CommandLine;
using CsvHelper.Configuration;
using ExifLib;
using FileHelpers;
using FileHelpers.Options;
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

        [Option('i', "interactions", Default = "interactions.txt", Required = false, HelpText = "Set interaction csv file path.")]
        public string InteractionsPath { get; set; }
        
        [Option('p', "pos", Default = "complete.pos", Required = false, HelpText = "Set interaction csv file path.")]
        public string PositionPath { get; set; }
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
                throw new ArgumentException($"Path {'"'}{options.PositionPath}{'"'} does not exist");

            if (!File.Exists(options.PositionPath))
                throw new ArgumentException($"Path {'"'}{options.PositionPath}{'"'} does not exist");

            VerboseWriteLine($"Trying to read {options.InteractionsPath}.");

            List<InteractionInfo> interactions = null;
            using (TextReader textReader = File.OpenText(options.InteractionsPath))
            {
                var csvReader = new CsvHelper.CsvReader(textReader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
                interactions = csvReader.GetRecords<InteractionInfo>().ToList();
            }

            VerboseWriteLine($"Read successful.");

            VerboseWriteLine($"Trying to read {options.PositionPath}.");

            var engine = new FileHelperEngine<PosFileEntry>();
            var result = engine.ReadFile(options.PositionPath);

            VerboseWriteLine($"Read successful.");
        }
    }
}
