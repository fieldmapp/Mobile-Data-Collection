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
                fileStreamIn.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Translates the details of a project to the runtime system language
        /// </summary>
        /// <param name="project">Project to be translated</param>
        /// <returns>New project with details in correct language (or with hints that the detail is missing)</returns>
        public static Project TranslateProjectDetails(Project project)
        {
            var authors = Parser.GetCurrentLanguageStringFromJsonList(project.Authors, project.Languages);
            if (authors == "Unable to parse language from json")
            {
                authors = AppResources.noauthor;
            }

            var title = Parser.GetCurrentLanguageStringFromJsonList(project.Title, project.Languages);
            if (title == "Unable to parse language from json")
            {
                title = AppResources.notitle;
            }

            var description = Parser.GetCurrentLanguageStringFromJsonList(project.Description, project.Languages);
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
        /// Walks the elements of a content page and applies a supplied action. This is deterministic.
        /// </summary>
        /// <param name="pages">An enumerable of all pages whose elements should be walked through</param>
        /// <param name="actionToApply">Action that will be applied to each element.</param>
        public static void WalkElements(IEnumerable<ContentPage> pages, Action<View> actionToApply)
        {
            foreach (var view in WalkElements(pages))
            {
                actionToApply(view);
            }
        }


        /// <summary>
        /// Walks the elements of a content page and returns an IEnumerable of them. This is deerministic.
        /// </summary>
        /// <param name="pages">An enumerable of all pages whose elements should be walked through</param>
        /// <returns>An IEnumerable of the elements which is created deferredly.</returns>
        public static IEnumerable<View> WalkElements(IEnumerable<ContentPage> pages)
        {
            foreach (var page in pages)
            {
                foreach (var stack in page.Content.LogicalChildren.OfType<StackLayout>())
                {
                    foreach (var grid in stack.Children.OfType<Grid>())
                    {
                        foreach (var element in grid.Children)
                        {
                            yield return element;
                        }
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// Creates the unique table name of a project. This is based on the title and the id.
        /// </summary>
        /// <param name="project">Project of which the table name should be retured.</param>
        /// <returns>Unique table name for the given project.</returns>
        public static string GetTableName(this Project project) => Parser.GetEnglishStringFromJsonList(project.Title, project.Languages) + "_" + project.Id;

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
    }
}
