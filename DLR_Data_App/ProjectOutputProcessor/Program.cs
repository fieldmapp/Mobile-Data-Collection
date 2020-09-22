using CommandLine;
using ExifLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOutputProcessor
{
    public class Options
    {
        [Option('v', "verbose", Default = false, Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('i', "input-path", Default = "input.txt", Required = false, HelpText = "Set input json file path.")]
        public string InputPath { get; set; }

        [Option('o', "output-path", Default = "output.txt", Required = false, HelpText = "Set output json file path.")]
        public string OutputPath { get; set; }

        [Option('f', "images-folder-path", Default = "img/", Required = false, HelpText = "Set path to output directory of extracted images.")]
        public string ImagesPath { get; set; }

        [Option('m', "min-base64-length", Default = 200, Required = false, HelpText = "Set minimum length of strings to be inspected for base64 content.")]
        public int MinBase64Lenght { get; set; }

        [Option('t', "temp-file-name", Default = "current", Required = false, HelpText = "Set path to use for the temp file.")]
        public string TempFilePath { get; set; }

        [Option('d', "date-format", Default = "yyyyMMdd_HHmmss", Required = false, HelpText = "Set format for dates in image names. See C# DateTime.ToString docs for more info.")]
        public string DateFormat { get; set; }
    }
    class Program
    {
        static bool DecodeBase64(string s, int start, int length, out byte[] result)
        {
            var base64Span = s.AsSpan(start, length);
            result = new byte[Helpers.GetOriginalLengthInBytes(base64Span)];
            return Convert.TryFromBase64Chars(base64Span, result, out int _);
        }

        static async Task Main(string[] args)
        {
            bool notParsed = false;
            Options options = null;

            void VerboseWriteLine(string s)
            {
                if (options.Verbose)
                    Console.WriteLine(s);
            }

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => options = o)
                .WithNotParsed(o => notParsed = true);

            if (notParsed)
                return;

            if (options == null)
                throw new ArgumentException("arguments given are malformed");

            if (!File.Exists(options.InputPath))
                throw new ArgumentException($"Given input path {'"'}{options.InputPath}{'"'} does not exist");

            if (!Directory.Exists(options.ImagesPath))
            {
                VerboseWriteLine($"Output path {options.ImagesPath} does not exist. Creating...");
                Directory.CreateDirectory(options.ImagesPath);
                VerboseWriteLine("Successful.");
            }

            VerboseWriteLine($"Trying to read {options.InputPath}.");

            string[] databaseOutput = await File.ReadAllLinesAsync(options.InputPath);

            VerboseWriteLine($"Read successful.");
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var lineWithIndex in databaseOutput.Select((l,i) => new { Line = l, Index = i }))
            {
                VerboseWriteLine($"Processing line {lineWithIndex.Index + 1}.");

                var indices = lineWithIndex.Line.AllIndexesOf('"').ToList();
                VerboseWriteLine($" Found {indices.Count} quotation marks.");
                if (indices.Count == 0)
                    continue;

                if (indices.Count % 2 == 1)
                    throw new Exception("Malformed json file");

                List<ReadOnlyMemory<char>> line_parts_reversed = new List<ReadOnlyMemory<char>>();
                line_parts_reversed.Add(lineWithIndex.Line.AsMemory(indices[^1]));

                for (int i = indices.Count - 2; i >= 0; i -= 2)
                {
                    VerboseWriteLine($" Processing string from index {indices[i]} to index {indices[i + 1]}.");
                    var stringLength = indices[i + 1] - indices[i] - 1;
                    if (stringLength >= options.MinBase64Lenght)
                    {
                        if (DecodeBase64(lineWithIndex.Line, indices[i] + 1, stringLength, out byte[] result))
                        {
                            VerboseWriteLine($" Found base64 content. Trying to write to {options.TempFilePath}.");
                            await File.WriteAllBytesAsync(options.TempFilePath, result);
                            VerboseWriteLine($" Write successful. Inspecting file for exif date...");
                            bool dateFound = false;
                            DateTime datePictureTaken;
                            using (ExifReader reader = new ExifReader(options.TempFilePath))
                            {
                                dateFound = reader.GetTagValue(ExifTags.DateTimeDigitized, out datePictureTaken);
                            }
                            if (dateFound)
                            {
                                var file_name = $"photo_{datePictureTaken.ToString(options.DateFormat, CultureInfo.InvariantCulture)}.jpg";
                                var targetPath = Path.Combine(options.ImagesPath, file_name);
                                VerboseWriteLine($" Exif date found. Moving file to {targetPath} and marking in output.");
                                line_parts_reversed.Add(file_name.AsMemory());
                                line_parts_reversed.Add("\"".AsMemory());
                                File.Move(options.TempFilePath, targetPath, true);
                                VerboseWriteLine(" Move successful.");
                            }
                            else
                            {
                                VerboseWriteLine(" No exif date found. Deleting file and marking so in output.");
                                File.Delete(options.TempFilePath);
                                line_parts_reversed.Add("removed_malformed_file".AsMemory());
                                line_parts_reversed.Add("\"".AsMemory());
                            }
                        }
                        else
                        {
                            VerboseWriteLine(" String is long enough to be inspected for base64 content, but it does not match base64 format.");
                            line_parts_reversed.Add(lineWithIndex.Line.AsMemory(indices[i], stringLength + 1));
                        }
                    }
                    else
                    {
                        VerboseWriteLine(" String to short to be inspected for base64 content. You can change this bound by setting min-base64-length");
                        line_parts_reversed.Add(lineWithIndex.Line.AsMemory(indices[i], stringLength + 1));
                    }

                    //enter part between two strings
                    int endIndexOfNextLeftPart = i == 0 ? 0 : indices[i - 1];
                    int length = indices[i] - endIndexOfNextLeftPart;
                    line_parts_reversed.Add(lineWithIndex.Line.AsMemory(endIndexOfNextLeftPart, length));
                }

                for (int i = line_parts_reversed.Count - 1; i >= 0; i--)
                {
                    stringBuilder.Append(line_parts_reversed[i]);
                }
                databaseOutput[lineWithIndex.Index] = stringBuilder.ToString();
                stringBuilder.Clear();
            }
            VerboseWriteLine($"Finished extracting images. Trying to write output into {options.OutputPath}.");
            File.WriteAllLines(options.OutputPath, databaseOutput);
        }
    }
}
