using System;
using System.Numerics;
using Xamarin.Essentials;

namespace DlrDataApp.Modules.SharedModule.Services.Sensors
{
    public class Gyroscope
    {
        public event EventHandler<GyroscopeChangedEventArgs> ReadingChanged
        {
            add => Xamarin.Essentials.Gyroscope.ReadingChanged += value;
            remove => Xamarin.Essentials.Gyroscope.ReadingChanged -= value;
        }

        public Vector3 Current { get; private set; }
        public float CurrentX => Current.X;
        public float CurrentY => Current.Y;
        public float CurrentZ => Current.Z;
        public float MaxX { get; private set; }
        public float MaxY { get; private set; }
        public float MaxZ { get; private set; }
        
        public Gyroscope()
        {
            Reset();
        }

        /// <summary>
        /// EventHandler which collects values after each change
        /// </summary>
        public void Reading_Changed(object sender, GyroscopeChangedEventArgs e)
        {
            var data = e.Reading;

            Current = data.AngularVelocity;

            MaxX = Math.Max(MaxX, Math.Abs(CurrentX));
            MaxY = Math.Max(MaxY, Math.Abs(CurrentY));
            MaxZ = Math.Max(MaxZ, Math.Abs(CurrentZ));
        }

        /// <summary>
        /// Resets values.
        /// </summary>
        public void Reset()
        {
            Current = Vector3.Zero;
            MaxX = 0.0F;
            MaxY = 0.0F;
            MaxZ = 0.0F;
        }
    }
}
