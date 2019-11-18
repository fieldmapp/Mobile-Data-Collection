using System;

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

        public float CurrentX { get; set; }
        public float CurrentY { get; set; }
        public float CurrentZ { get; set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }
        public float MaxZ { get; set; }
        
        public Magnetometer()
        {
            Reset();
        }


        /// <summary>
        /// EventHandler which collects values after each change
        /// </summary>
        public void Reading_Changed(object sender, MagnetometerChangedEventArgs e)
        {
            var data = e.Reading;

            CurrentX = data.MagneticField.X;
            CurrentY = data.MagneticField.Y;
            CurrentZ = data.MagneticField.Z;

            if (Math.Abs(CurrentX) > MaxX)
            {
                MaxX = Math.Abs(CurrentX);
            }

            if (Math.Abs(CurrentY) > MaxY)
            {
                MaxY = Math.Abs(CurrentY);
            }

            if (Math.Abs(CurrentZ) > MaxZ)
            {
                MaxZ = Math.Abs(CurrentZ);
            }
        }

        /// <summary>
        /// Resets values.
        /// </summary>
        public void Reset()
        {
            CurrentX = 0.0F;
            CurrentY = 0.0F;
            CurrentZ = 0.0F;
            MaxX = 0.0F;
            MaxY = 0.0F;
            MaxZ = 0.0F;
        }
    }
}
