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
        public static async Task PushPage(this INavigation navigation, Page page, bool animated = true)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                await navigation.PushAsync(page, animated);
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                await navigation.PushModalAsync(page, animated);
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

        public static string GetSafeIdentifier(this DateTime dateTime)
        {
            return dateTime.ToString("ddMMyyyyHHmmss", CultureInfo.InvariantCulture);
        }

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
        /// Extension for 'Object' that copies the properties to a destination object. Taken from https://stackoverflow.com/a/8724150
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


        public const string BoldMarker = "*";

        public static FormattedString StringWithAnnotationsToFormattedString(string annotatedInput)
        {

            if (string.IsNullOrWhiteSpace(annotatedInput))
                return new FormattedString();

            annotatedInput = annotatedInput.Replace("\\n", Environment.NewLine);
            var markerIndices = annotatedInput.AllIndicesOf(BoldMarker);
            int prevPartEndIndex = 0;
            var result = new FormattedString();

            bool bold = false;
            foreach (var markerIndex in markerIndices.Concat(new int[] { annotatedInput.Length }))
            {
                var part = annotatedInput.Substring(prevPartEndIndex, markerIndex - prevPartEndIndex);
                if (!string.IsNullOrWhiteSpace(part))
                {
                    var span = new Span { Text = part };
                    if (bold)
                        span.FontAttributes = FontAttributes.Bold;

                    result.Spans.Add(span);
                }

                bold = !bold;
                prevPartEndIndex = markerIndex + 1;
            }

            return result;
        }

        public static string FormattedStringToAnnotatedString(FormattedString formattedString)
        {
            if (formattedString == null)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            foreach (var span in formattedString.Spans)
            {
                if (span.FontAttributes.HasFlag(FontAttributes.Bold))
                    builder.Append('*');
                builder.Append(span.Text);
                if (span.FontAttributes.HasFlag(FontAttributes.Bold))
                    builder.Append('*');
            }
            return builder.ToString().Replace(Environment.NewLine, "\\n");
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
    }
}
