using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using System.Linq;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Windows.Input;

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
            s = s.ToLower();
            t = t.ToLower();
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

        public static bool Overrides(this Type type, MethodInfo baseMethod)
        {
            if (baseMethod == null)
                throw new ArgumentNullException("baseMethod");
            if (type == null)
                throw new ArgumentNullException("type");
            if (!type.IsSubclassOf(baseMethod.ReflectedType))
                throw new ArgumentException(string.Format("Type must be subtype of {0}", baseMethod.DeclaringType));
            while (type != baseMethod.ReflectedType)
            {
                var methods = type.GetMethods(BindingFlags.Instance |
                                            BindingFlags.DeclaredOnly |
                                            BindingFlags.Public |
                                            BindingFlags.NonPublic);
                if (methods.Any(m => m.GetBaseDefinition() == baseMethod))
                    return true;
                type = type.BaseType;
            }
            return false;
        }
        
        public static async Task WaitOneAsync(this WaitHandle waitHandle) => await Task.Run(() => waitHandle.WaitOne());

        private static string[] DigitToString = new string[]
        {
            "null", "eins", "zwei", "drei", "vier",
            "fünf", "sechs", "sieben", "acht", "neun"
        };
        public static string GetStringFromDigit(this int digit)
        {
            if (digit > 9 || digit < 0)
                throw new ArgumentException("param must be in rang 0 to 9 (inclusive)", nameof(digit));
            return DigitToString[digit];
        }
    }
}
