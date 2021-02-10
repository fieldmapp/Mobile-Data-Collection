using System;
using Xamarin.Essentials;

namespace DlrDataApp.Modules.SharedModule.Services.Sensors
{
    public class Barometer
    {
        public event EventHandler<BarometerChangedEventArgs> ReadingChanged
        {
            add => Xamarin.Essentials.Barometer.ReadingChanged += value;
            remove => Xamarin.Essentials.Barometer.ReadingChanged -= value;
        }

        public double CurrentPressure { get; set; }
        public double MaxPressure { get; set; }

        public Barometer()
        {
            Reset();
        }

        /// <summary>
        /// EventHandler which collects values after each change
        /// </summary>
        public void Reading_Changed(object sender, BarometerChangedEventArgs e)
        {
            var data = e.Reading;

            CurrentPressure = data.PressureInHectopascals;

            if (CurrentPressure > MaxPressure)
            {
                MaxPressure = CurrentPressure;
            }
        }

        /// <summary>
        /// Resets values.
        /// </summary>
        public void Reset()
        {
            CurrentPressure = 0.0;
            MaxPressure = 0.0;
        }
    }
}
