using ExifLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOutputProcessor
{
    class Program
    {
        const int MinBase64Length = 200;
        const string WorkingFileName = "current.jpg";

        static bool DecodeBase64(string s, int start, int length, out byte[] result)
        {
            var base64Span = s.AsSpan(start, length);
            result = new byte[Helpers.GetOriginalLengthInBytes(base64Span)];
            return Convert.TryFromBase64Chars(base64Span, result, out int _);
        }

        static async Task Main(string[] args)
        {
            string[] databaseOutput = await File.ReadAllLinesAsync("input.txt");

            foreach (var lineWithIndex in databaseOutput.Select((l,i) => new { Value = l, Index = i }))
            {
                var indices = lineWithIndex.Value.AllIndexesOf('"').ToList();
                if (indices.Count == 0)
                    continue;

                if (indices.Count % 2 == 1)
                    throw new Exception("Malformed json file");

                List<ReadOnlyMemory<char>> line_parts_reversed = new List<ReadOnlyMemory<char>>();
                line_parts_reversed.Add(lineWithIndex.Value.AsMemory(indices[^1]));

                for (int i = indices.Count - 2; i >= 0; i -= 2)
                {
                    var stringLength = indices[i + 1] - indices[i] - 1;
                    if (stringLength >= MinBase64Length)
                    {
                        if (DecodeBase64(lineWithIndex.Value, indices[i] + 1, stringLength, out byte[] result))
                        {
                            await File.WriteAllBytesAsync(WorkingFileName, result);
                            bool dateFound = false;
                            DateTime datePictureTaken;
                            using (ExifReader reader = new ExifReader(WorkingFileName))
                            {
                                dateFound = reader.GetTagValue(ExifTags.DateTimeDigitized, out datePictureTaken);
                            }
                            if (dateFound)
                            {
                                var file_name = $"img_{datePictureTaken:yyyyMMdd_HHmmss}.jpg";
                                line_parts_reversed.Add(file_name.AsMemory());
                                line_parts_reversed.Add("\"".AsMemory());
                                File.Move(WorkingFileName, file_name, true);
                            }
                            else
                            {
                                File.Delete(WorkingFileName);
                                line_parts_reversed.Add($"{'"'}removed_malformed_file".AsMemory());
                            }
                        }
                        else
                        {
                            line_parts_reversed.Add(lineWithIndex.Value.AsMemory(indices[i], stringLength + 1));
                        }
                    }
                    else
                    {
                        line_parts_reversed.Add(lineWithIndex.Value.AsMemory(indices[i], stringLength + 1));
                    }

                    //enter part between two strings
                    int endIndexOfNextLeftPart = i == 0 ? 0 : indices[i - 1];
                    int length = indices[i] - endIndexOfNextLeftPart;
                    line_parts_reversed.Add(lineWithIndex.Value.AsMemory(endIndexOfNextLeftPart, length));
                }


                StringBuilder sb = new StringBuilder();
                for (int i = line_parts_reversed.Count - 1; i >= 0; i--)
                {
                    sb.Append(line_parts_reversed[i]);
                }
                databaseOutput[lineWithIndex.Index] = sb.ToString();
            }
            File.WriteAllLines("output.txt", databaseOutput);
        }
    }
}
