using System;
using System.Collections.Generic;
using System.Text;

namespace FieldCartographerProcessor
{
    internal static class Helpers
    {
        public static IEnumerable<int> AllIndexesOf(this string str, string value)
        {
            // from https://stackoverflow.com/a/2641383

            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }
        public static IEnumerable<int> AllIndexesOf(this string str, char value)
        {
            for (int index = 0; ; index += 1)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }

        public static int GetOriginalLengthInBytes(ReadOnlySpan<char> base64string)
        {
            // altered from https://blog.aaronlenoir.com/2017/11/10/get-original-length-from-base-64-string/ to work with spans
            if (base64string.Length == 0)
                return 0;

            var characterCount = base64string.Length;
            int paddingCount = 0;
            if (base64string[^1] == '=')
                paddingCount++;
            if (base64string[^2] == '=')
                paddingCount++;

            return (3 * (characterCount / 4)) - paddingCount;
        }
    }
}
