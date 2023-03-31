using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    /// <summary>
    /// Provides different static (mostly extension) methods which aid in different common scenarios.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Sets the content of a <see cref="ObservableCollection"/> to a new content after clearing the old. Does not break Bindings.
        /// </summary>
        /// <typeparam name="T">Type of elements in the collection.</typeparam>
        /// <param name="observableCollection">Collection which will get its content changed</param>
        /// <param name="newContent">New Content which will be filled into the Collection</param>
        public static void SetTo<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> newContent)
        {
            observableCollection.Clear();
            foreach (var item in newContent)
            {
                observableCollection.Add(item);
            }
        }

        /// <summary>
        /// Extracts files from zip file
        /// </summary>
        /// <param name="zipFilePath">Path of zip file</param>
        /// <param name="unzipFolderPath">Path to folder which should contain the extracted files</param>
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

        /// <summary>
        /// Extracts files from zip folder
        /// </summary>
        /// <param name="fileStreamIn">Stream reading a zip file</param>
        /// <param name="unzipFolderPath">Path to folder which should contain the extracted files</param>
        /// <returns>A task that represents the completion of unzipping</returns>
        /// <see cref="https://stackoverflow.com/questions/42118378/how-to-unzip-downloaded-zip-file-in-xamarin-forms"/>
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

        /// <summary>
        /// Returns if value is greater than other.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <typeparam name="U">Type of other</typeparam>
        public static bool IsGreaterThan<T, U>(this T value, U other) where T : IComparable<U>
        {
            return value.CompareTo(other) > 0;
        }


        /// <summary>
        /// Returns if value is less than other.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <typeparam name="U">Type of other</typeparam>
        public static bool IsLessThan<T, U>(this T value, U other) where T : IComparable<U>
        {
            return value.CompareTo(other) < 0;
        }

        /// <summary>
        /// Returns if value equals or is greater than other.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <typeparam name="U">Type of other</typeparam>
        public static bool IsGreaterThanOrEquals<T, U>(this T value, U other) where T : IComparable<U>
        {
            return value.CompareTo(other) >= 0;
        }

        /// <summary>
        /// Returns if value equals or is less than other.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <typeparam name="U">Type of other</typeparam>
        public static bool IsLessThanOrEquals<T, U>(this T value, U other) where T : IComparable<U>
        {
            return value.CompareTo(other) <= 0;
        }

        /// <summary>
        /// Calls the private Method <see cref="VisualElement.InvalidateMeasure"/> via reflection.
        /// </summary>
        /// <param name="view">Object that the method will be called on</param>
        public static void InvalidateSize(this View view)
        {
            if (view != null)
            {
                Type viewType = typeof(VisualElement);
                IEnumerable<MethodInfo> methods = viewType.GetTypeInfo()
                                                          .DeclaredMethods;
                MethodInfo method = methods.FirstOrDefault(m => m.Name == "InvalidateMeasure");

                if (method != null)
                {
                    method.Invoke(view, null);
                }
            }
        }

        /// <summary>
        /// Converts a given <see cref="DateTime"/> into a iso 8601 timestap string.
        /// </summary>
        /// <param name="dateTime">Timestamp which will be converted to a string</param>
        /// <returns>ISO 8601 timestamp.</returns>
        public static string GetSafeIdentifier(this DateTime dateTime)
        {
            return dateTime.ToString("s").Replace(":", "_");
        }

        /// <summary>
        /// Finds all occurances of a string in another string.
        /// </summary>
        /// <param name="str">String which will get searched in</param>
        /// <param name="value">String which will get searched for</param>
        /// <returns>List of all indices where <paramref name="value"/> starts in <paramref name="str"/></returns>
        public static List<int> AllIndicesOf(this string str, string value)
        {
            // taken from https://stackoverflow.com/a/2641383/8512719
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        /// <summary>
        /// Extension for 'Object' that copies the properties to a destination object. 
        /// Taken from https://stackoverflow.com/a/8724150
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyProperties(this object source, object destination)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();

            // Iterate the Properties of the source instance and  
            // populate them from their desination counterparts  
            PropertyInfo[] srcProps = typeSrc.GetProperties();
            foreach (PropertyInfo srcProp in srcProps)
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if (targetProperty.GetSetMethod(true) != null || targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }
                // Passed all tests, lets set the value
                targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
            }
        }

        /// <summary>
        /// Performs a click/a tap on a given button.
        /// Taken from  https://stackoverflow.com/a/60297754
        /// </summary>
        public static void PerformClick(this Button sourceButton)
        {

            // Check parameters
            if (sourceButton == null)
                throw new ArgumentNullException(nameof(sourceButton));

            // 1.) Raise the Click-event
#if !NETSTANDARD
            sourceButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
#else
            sourceButton.RaiseEventViaReflection(nameof(sourceButton.Clicked), EventArgs.Empty);
#endif

            // 2.) Execute the command, if bound and can be executed
            ICommand boundCommand = sourceButton.Command;
            if (boundCommand != null)
            {
                object parameter = sourceButton.CommandParameter;
                if (boundCommand.CanExecute(parameter) == true)
                    boundCommand.Execute(parameter);
            }
        }

#if NETSTANDARD
        /// <summary>
        /// Invokes a Event via reflection.
        /// Taken from https://stackoverflow.com/a/60297754
        /// </summary>
        public static void RaiseEventViaReflection<TEventArgs>(this object source, string eventName, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            var eventDelegate = (MulticastDelegate)source.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(source);
            if (eventDelegate == null)
                return;
            foreach (var handler in eventDelegate.GetInvocationList())
            {
#if !(NETSTANDARD1_6 || NETSTANDARD1_5 || NETSTANDARD1_4 || NETSTANDARD1_3 || NETSTANDARD1_2 || NETSTANDARD1_1 || NETSTANDARD1_0)
                handler.Method?.Invoke(handler.Target, new object[] { source, eventArgs });
#else
               handler.GetMethodInfo()?.Invoke(handler.Target, new object[] { source, eventArgs });
#endif
            }
        }
#endif



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
        /// Copies a directory to a new path. Taken from https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories 
        /// </summary>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, bool ignoreNonExistentSource = true)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
            {
                if (ignoreNonExistentSource)
                    return;
                else
                    throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        /// <summary>
        /// Reverses an given input dictionary.
        /// </summary>
        public static Dictionary<K, List<V>> ReverseDictionary<K, V>(this Dictionary<V, K> input)
        {
            return input.GroupBy(p => p.Value)
                .ToDictionary(g => g.Key, g => g.Select(pp => pp.Key).ToList());
        }

        public static Dictionary<TKey, TElement> ToDictionaryAllowDuplicates<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer = null)
        {
            // taken from https://stackoverflow.com/a/22508992
            var dictionary = new Dictionary<TKey, TElement>(comparer);

            if (source == null)
            {
                return dictionary;
            }

            foreach (TSource element in source)
            {
                dictionary[keySelector(element)] = elementSelector(element);
            }

            return dictionary;
        }
    }
}
