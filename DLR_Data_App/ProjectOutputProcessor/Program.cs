using ExifLib;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProjectOutputProcessor
{
    class Program
    {
        const string WorkingFileName = "current.jpg";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string imageBase64 = (await File.ReadAllLinesAsync("base64.txt"))[0];

            await File.WriteAllBytesAsync(@"current.jpg", Convert.FromBase64String(imageBase64));

            bool dateFound = false;
            DateTime datePictureTaken;
            using (ExifReader reader = new ExifReader(WorkingFileName))
            {
                dateFound = reader.GetTagValue(ExifTags.DateTimeDigitized, out datePictureTaken);
            }
            if (dateFound)
                File.Move(WorkingFileName, $"img_{datePictureTaken:yyyyMMdd_HHmmss}.jpg");
        }
    }
}
