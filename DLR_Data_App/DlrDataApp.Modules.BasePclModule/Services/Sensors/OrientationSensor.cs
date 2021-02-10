using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Xamarin.Essentials;

namespace DlrDataApp.Modules.SharedModule.Services.Sensors
{
    public class OrientationSensor
    {
        public event EventHandler<OrientationSensorChangedEventArgs> ReadingChanged
        {
            add => Xamarin.Essentials.OrientationSensor.ReadingChanged += value;
            remove => Xamarin.Essentials.OrientationSensor.ReadingChanged -= value;
        }

        public OrientationSensor()
        {
            Reset();
        }

        public Quaternion Orientation;


        /// <summary>
        /// EventHandler which collects values after each change
        /// </summary>
        public void Reading_Changed(object sender, OrientationSensorChangedEventArgs e)
        {
            Orientation = e.Reading.Orientation;
        }

        /// <summary>
        /// Resets values.
        /// </summary>
        public void Reset()
        {
            Orientation = Quaternion.Identity;
        }
    }
}
