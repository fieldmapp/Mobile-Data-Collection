using System;
using Xamarin.Essentials;

namespace DlrDataApp.Modules.Base.Shared.Services.Sensors
{
    public class Compass
    {
        public event EventHandler<CompassChangedEventArgs> ReadingChanged
        {
            add => Xamarin.Essentials.Compass.ReadingChanged += value;
            remove => Xamarin.Essentials.Compass.ReadingChanged -= value;
        }

        public double Degrees { get; set; }
        public string Direction { get; set; }

        public Compass()
        {
            Reset();
        }

        /// <summary>
        /// Collects values after each change.
        /// </summary>
        public void Reading_Changed(object sender, CompassChangedEventArgs e)
        {
            var data = e.Reading;

            Degrees = data.HeadingMagneticNorth;
            CalcDirection();
        }

        /// <summary>
        /// Resets values.
        /// </summary>
        public void Reset()
        {
            Degrees = 0.0;
            Direction = "N";
        }

        /// <summary>
        /// Generates direction from angle of degrees.
        /// </summary>
        private void CalcDirection()
        {
            if (Degrees > 337.5 || Degrees < 22.5)
            {
                Direction = "N";
            }

            if (Degrees >= 22.5 && Degrees < 67.5)
            {
                Direction = "NE";
            }

            if (Degrees >= 67.5 && Degrees < 112.5)
            {
                Direction = "E";
            }

            if (Degrees >= 112.5 && Degrees < 157.5)
            {
                Direction = "SE";
            }

            if (Degrees >= 157.5 && Degrees < 202.5)
            {
                Direction = "S";
            }

            if (Degrees >= 202.5 && Degrees < 247.5)
            {
                Direction = "SW";
            }

            if (Degrees >= 247.5 && Degrees < 292.5)
            {
                Direction = "W";
            }

            if (Degrees >= 292.5 && Degrees < 337.5)
            {
                Direction = "NW";
            }
        }
    }
}
