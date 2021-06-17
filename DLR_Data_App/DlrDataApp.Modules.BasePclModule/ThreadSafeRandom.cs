using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.Base.Shared
{
    /// <summary>
    /// Thread safe wrapper around <see cref="System.Random"/>. Inspired from https://stackoverflow.com/a/11109361/8512719. Doesn't override <see cref="System.Random.Sample"/>.
    /// </summary>
    public class ThreadSafeRandom : Random
    {
        private static readonly Random _globalRandom = new Random();
        [ThreadStatic] private static Random _localRandom;

        public override int Next()
        {
            EnsureLocalRandomSet();

            return _localRandom.Next();
        }

        public override int Next(int maxValue)
        {
            EnsureLocalRandomSet();

            return _localRandom.Next(maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            EnsureLocalRandomSet();

            return _localRandom.Next(minValue, maxValue);
        }

        public override void NextBytes(byte[] buffer)
        {
            EnsureLocalRandomSet();

            _localRandom.NextBytes(buffer);
        }

        public override double NextDouble()
        {
            EnsureLocalRandomSet();

            return _localRandom.NextDouble();
        }

        private static void EnsureLocalRandomSet()
        {
            if (_localRandom == null)
            {
                lock (_globalRandom)
                {
                    if (_localRandom == null)
                    {
                        int seed = _globalRandom.Next();
                        _localRandom = new Random(seed);
                    }
                }
            }
        }
    }
}
