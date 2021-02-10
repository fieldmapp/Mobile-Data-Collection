using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.SharedModule
{
    public static class Helpers
    {
        public static void SetTo<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> newContent)
        {
            observableCollection.Clear();
            foreach (var item in newContent)
            {
                observableCollection.Add(item);
            }
        }

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
                await navElement.Navigation.PushAsync(page, animated);
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                await navElement.Navigation.PushModalAsync(page, animated);
            }
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

        public static bool IsGreaterThan<T>(this T value, T other) where T : IComparable<T>
        {
            return value.CompareTo(other) > 0;
        }

        public static bool IsLessThan<T>(this T value, T other) where T : IComparable<T>
        {
            return value.CompareTo(other) < 0;
        }

        public static bool IsGreaterThanOrEquals<T>(this T value, T other) where T : IComparable<T>
        {
            return value.CompareTo(other) >= 0;
        }

        public static bool IsLessThanOrEquals<T>(this T value, T other) where T : IComparable<T>
        {
            return value.CompareTo(other) <= 0;
        }


    }
}
