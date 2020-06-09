using System;
using System.Numerics;
using Xamarin.Essentials;

namespace DLR_Data_App.Services.Sensors
{
    public class Magnetometer
    {
        public event EventHandler<MagnetometerChangedEventArgs> ReadingChanged
        {
            add => Xamarin.Essentials.Magnetometer.ReadingChanged += value;
            remove => Xamarin.Essentials.Magnetometer.ReadingChanged -= value;
        }

        public Vector3 Current { get; private set; }

        public float CurrentX => Current.X;
        public float CurrentY => Current.Y;
        public float CurrentZ => Current.Z;
        public float MaxX { get; private set; }
        public float MaxY { get; private set; }
        public float MaxZ { get; private set; }
        
        public Magnetometer()
        {
            Reset();
        }


        /// <summary>
        /// EventHandler which collects values after each change
        /// </summary>
        public void Reading_Changed(object sender, MagnetometerChangedEventArgs e)
        {
            Current = e.Reading.MagneticField;

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
