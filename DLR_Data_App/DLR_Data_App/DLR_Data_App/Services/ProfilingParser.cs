using DLR_Data_App.Models.Profiling;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLR_Data_App.Services
{
    public static class ProfilingParser
    {
        const string ProfilingJsonFileName = "Profiling.json";

        /// <summary>
        /// Executes all steps to unpack zip archive and add forms to project.
        /// </summary>
        /// <param name="zipFile">Path to ZIP archive</param>
        /// <param name="unzipFolder">Path to extract folder</param>
        public static async Task<ProfilingData> ParseZip(string zipFile, string unzipFolder)
        {
            // Extract zip archive
            if (!await Helpers.UnzipFileAsync(zipFile, unzipFolder))
            {
                return null;
            }

            // Get files
            var unzipContent = Directory.GetFiles(unzipFolder);
            var profilingJsonPath = unzipContent.FirstOrDefault(f => f.EndsWith(Path.PathSeparator + ProfilingJsonFileName));
            if (profilingJsonPath == null)
            {
                return null;
            }

            var profilingJsonContent = File.ReadAllText(profilingJsonPath);
            try
            {
                return JsonTranslator.GetFromJson<ProfilingData>(profilingJsonContent);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
