using DLR_Data_App.Models.ProjectModel;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DLR_Data_App.Localizations;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using System.Linq;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Numerics;

namespace DLR_Data_App.Services
{
    /// <summary>
    /// Static class containing both normal and extension methods. 
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Hashes passphrases in SHA512
        /// </summary>
        /// <param name="input">String to hash</param>
        /// <returns>Hashed string</returns>
        /// <see cref="https://docs.microsoft.com/de-de/dotnet/api/system.security.cryptography.hashalgorithm.computehash?view=netframework-4.8#System_Security_Cryptography_HashAlgorithm_ComputeHash_System_Byte__"/>
        /// <see cref="https://docs.microsoft.com/de-de/dotnet/api/system.security.cryptography.sha512?view=netframework-4.8"/>
        public static string CalculateSHA512Hash(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            byte[] resultBytes;
            var sBuilder = new StringBuilder();

            using (SHA512 shaM = new SHA512Managed())
            {
                resultBytes = shaM.ComputeHash(data);
            }

            foreach (var t in resultBytes)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            return sBuilder.ToString();
        }
        
        /// <summary>
        /// Extracts files from zip folder
        /// </summary>
        /// <param name="zipFilePath">Path of zip folder</param>
        /// <param name="unzipFolderPath">Path of extracted files</param>
        /// <returns>A task that represents the completion of unzipping</returns>
        /// <see cref="https://stackoverflow.com/questions/42118378/how-to-unzip-downloaded-zip-file-in-xamarin-forms"/>
        public static async Task<bool> UnzipFileAsync(string zipFilePath, string unzipFolderPath)
        {
            try
            {
                var fileStreamIn = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);
                await UnzipFileAsync(fileStreamIn, unzipFolderPath);
                fileStreamIn.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> UnzipFileAsync(Stream fileStreamIn, string unzipFolderPath)
        {
            var zipInStream = new ZipInputStream(fileStreamIn);
            var entry = zipInStream.GetNextEntry();
            if (Directory.Exists(unzipFolderPath))
                Directory.Delete(unzipFolderPath, true);

            while (entry != null && entry.CanDecompress)
            {
                var outputFile = unzipFolderPath + @"/" + entry.Name;
                var outputDirectory = Path.GetDirectoryName(outputFile);

                if (outputDirectory != null)
                {
                    if (!Directory.Exists(outputDirectory))
                        Directory.CreateDirectory(outputDirectory);
                }
                else
                {
                    return false;
                }

                if (entry.IsFile)
                {
                    var fileStreamOut = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                    int size;
                    byte[] buffer = new byte[4096];
                    do
                    {
                        size = await zipInStream.ReadAsync(buffer, 0, buffer.Length);
                        await fileStreamOut.WriteAsync(buffer, 0, size);
                    } while (size > 0);
                    fileStreamOut.Close();
                }

                entry = zipInStream.GetNextEntry();
            }
            zipInStream.Close();
            return true;
        }

        /// <summary>
        /// Translates the details of a project to the runtime system language
        /// </summary>
        /// <param name="project">Project to be translated</param>
        /// <returns>New project with details in correct language (or with hints that the detail is missing)</returns>
        public static Project TranslateProjectDetails(Project project)
        {
            var authors = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(project.Authors, project.Languages);
            if (authors == "Unable to parse language from json")
            {
                authors = AppResources.noauthor;
            }

            var title = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(project.Title, project.Languages);
            if (title == "Unable to parse language from json")
            {
                title = AppResources.notitle;
            }

            var description = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(project.Description, project.Languages);
            if (description == "Unable to parse language from json")
            {
                description = AppResources.nodescription;
            }

            var tempProject = new Project
            {
                Authors = authors,
                Title = title,
                Description = description
            };


            return tempProject;
        }

        /// <summary>
        /// Translates the details of multiple projects to the runtime system language
        /// </summary>
        /// <param name="project">Projects to be translated</param>
        /// <returns>New projects with details in correct language (or with hints that the detail is missing)</returns>
        public static List<Project> TranslateProjectDetails(List<Project> projectList) => projectList.Select(TranslateProjectDetails).ToList();

        /// <summary>
        /// Pushes a new page to the navigation stack with respect to the operation system
        /// </summary>
        /// <param name="navElement">Navigation element with attached NavigationPage. When called from a page, its just "this".</param>
        /// <param name="page">New page that should be pushed to the navigation stack.</param>
        /// <param name="animated">Determines if the new page should appear in an animation.</param>
        public static async Task PushPage(this NavigableElement navElement, Page page, bool animated = true)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                await (Application.Current as App).Navigation.PushAsync(page, animated);
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                await navElement.Navigation.PushModalAsync(page, animated);
            }
        }

        /// <summary>
        /// Creates the unique table name of a project. This is based on the title and the id.
        /// </summary>
        /// <param name="project">Project of which the table name should be retured.</param>
        /// <returns>Unique table name for the given project.</returns>
        public static string GetTableName(this Project project) => OdkDataExtractor.GetEnglishStringFromJsonList(project.Title, project.Languages) + "_" + project.Id;

        /// <summary>
        /// Performs a lookup for the english translation for a given translationKey. Used by profilings.
        /// </summary>
        /// <param name="translations">Dictionary containing translations for keys</param>
        /// <param name="translationKey">Key to lookup</param>
        /// <returns>Translation or the string "translation missing" if there is no english translation</returns>
        public static string GetEnglishTranslation(Dictionary<string,string> translations, string translationKey)
        {
            const string englishLanguageExtension = "English";
            translationKey = translationKey + englishLanguageExtension;
            if (!translations.TryGetValue(translationKey, out string translation))
            {
                translation = "translation missing";
            }
            return translation;
        }

        /// <summary>
        /// Performs a lookup for the system languages translation for a given translationKey. 
        /// Falls back to <see cref="GetEnglishTranslation"/> if there is no translation in the current language. Used by profilings.
        /// </summary>
        /// <param name="translations">Dictionary containing translations for keys</param>
        /// <param name="translationKey">Key to lookup</param>
        /// <returns>Translation or the string "translation missing" if there is neither a translation in the current language nor in english</returns>
        public static string GetCurrentLanguageTranslation(Dictionary<string, string> translations, string translationKey)
        {
            string currentLanguageExtension = CultureInfo.CurrentUICulture.EnglishName;
            int firstSpaceInCurrentLanguageExtension = currentLanguageExtension.IndexOf(' ');
            if (firstSpaceInCurrentLanguageExtension != -1)
            {
                currentLanguageExtension = currentLanguageExtension.Substring(0, firstSpaceInCurrentLanguageExtension);
            }
            var currentLanguageTranslationKey = translationKey + currentLanguageExtension;
            if (!translations.TryGetValue(currentLanguageTranslationKey, out string translation))
            {
                translation = GetEnglishTranslation(translations, translationKey);
            }
            return translation;
        }

        /// <summary>
        /// Extension Method providing the IndexOf method for <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IReadOnlyList{T}"/></typeparam>
        /// <param name="list">List in which the given element will be searched for</param>
        /// <param name="element">Element which wil be looked for</param>
        /// <returns>Index of element in list or (if the element is not in list) -1.</returns>
        public static int IndexOf<T>(this IReadOnlyList<T> list, T element)
        {
            var listCount = list.Count;
            for (int i = 0; i < listCount; i++)
            {
                if (list[i].Equals(element))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is false and one after that.
        /// </summary>
        /// <typeparam name="T">The type of the element of source.</typeparam>
        /// <param name="list">A sequence to return elements from</param>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the elements from the input sequence that occur before and including the element at which the test passes.</returns>
        public static IEnumerable<T> TakeUntilIncluding<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            foreach (T el in list)
            {
                yield return el;
                if (predicate(el))
                    yield break;
            }
        }

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition is true, skips one and then returns the remaining elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="list">An <see cref="IEnumerable{T}"/> to return elements from</param>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the elements from the input sequence starting one after the first element in the linear series that does not pass the test specified by predicate</returns>
        public static IEnumerable<T> SkipWhileIncluding<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            bool yielding = false;
            foreach (T element in list)
            {
                if (yielding) yield return element;
                if (!yielding && !predicate(element)) yielding = true;
            }
        }

        public static void SetTo<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> newContent)
        {
            observableCollection.Clear();
            foreach (var item in newContent)
            {
                observableCollection.Add(item);
            }
        }

        public static Vector3 ToEulerAngles(this Quaternion q)
        {
            // from https://stackoverflow.com/q/11492299
            // Store the Euler angles in radians
            Vector3 pitchYawRoll = new Vector3();

            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;

            // If quaternion is normalised the unit is one, otherwise it is the correction factor
            double unit = sqx + sqy + sqz + sqw;
            double test = q.X * q.Y + q.Z * q.W;

            if (test > 0.4999f * unit)                              // 0.4999f OR 0.5f - EPSILON
            {
                // Singularity at north pole
                pitchYawRoll.Y = 2f * (float)Math.Atan2(q.X, q.W);  // Yaw
                pitchYawRoll.X = (float)Math.PI * 0.5f;             // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }
            else if (test < -0.4999f * unit)                        // -0.4999f OR -0.5f + EPSILON
            {
                // Singularity at south pole
                pitchYawRoll.Y = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
                pitchYawRoll.X = (float)-Math.PI * 0.5f;            // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }
            else
            {
                pitchYawRoll.Y = (float)Math.Atan2(2f * q.Y * q.W - 2f * q.X * q.Z, sqx - sqy - sqz + sqw);       // Yaw
                pitchYawRoll.X = (float)Math.Asin(2f * test / unit);                                              // Pitch
                pitchYawRoll.Z = (float)Math.Atan2(2f * q.X * q.W - 2f * q.Y * q.Z, -sqx + sqy - sqz + sqw);      // Roll
            }

            return pitchYawRoll;
        }

        public static double ToDegrees(this double radians)
        {
            return (180 / Math.PI) * radians;
        }

        public static float ToDegrees(this float radians)
        {
            return (180 / (float)Math.PI) * radians;
        }

        public static Vector3 PointwiseOperation(this Vector3 vec, Func<float, float> op)
        {
            return new Vector3(op(vec.X), op(vec.Y), op(vec.Z));
        }

        public static Vector3 PointwiseDoubleOperation(this Vector3 vec, Func<double, double> op)
        {
            return new Vector3((float)op(vec.X), (float)op(vec.Y), (float)op(vec.Z));
        }

        public static Vector3 Abs(this Vector3 vec) => vec.PointwiseOperation(Math.Abs);

        /// <summary>
        /// Calculates the levenshtein distance of two strings.
        /// Taken from https://www.dotnetperls.com/levenshtein
        /// </summary>
        public static int LevenshteinDistnace(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            //taken from https://stackoverflow.com/a/914198/8512719
            return source.MinBy(selector, null);
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            //taken from https://stackoverflow.com/a/914198/8512719
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            comparer = comparer ?? Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }
    }
}
