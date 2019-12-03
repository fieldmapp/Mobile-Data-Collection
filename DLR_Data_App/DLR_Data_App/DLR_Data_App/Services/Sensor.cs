using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DLR_Data_App.Services
{
    public class Sensor
    {
        public static Sensor Instance;

        private SensorSpeed _speed = SensorSpeed.UI;

        public Sensors.Accelerometer Accelerometer;
        public Sensors.Barometer Barometer;
        public Sensors.Compass Compass;
        public Sensors.Gps Gps;
        public Sensors.Gyroscope Gyroscope;
        public Sensors.Magnetometer Magnetometer;

        /// <summary>
        /// Checks for preference changes before opening the page.
        /// </summary>
        private void Init()
        {
            if (Preferences.Get("accelerometer", true))
            {
                if (!(Xamarin.Essentials.Accelerometer.IsMonitoring))
                {
                    try
                    {
                        Xamarin.Essentials.Accelerometer.Start(_speed);
                    }
                    catch (FeatureNotEnabledException) { }
                }
            }
            else
            {
                try
                {
                    Xamarin.Essentials.Accelerometer.Stop();
                }
                catch (FeatureNotEnabledException) { }

            }

            if (Preferences.Get("barometer", true))
            {
                if (!(Xamarin.Essentials.Barometer.IsMonitoring))
                {
                    try
                    {
                        Xamarin.Essentials.Barometer.Start(_speed);
                    }
                    catch (FeatureNotEnabledException) { }

                }
            }
            else
            {
                try
                {
                    Xamarin.Essentials.Barometer.Stop();
                }
                catch (FeatureNotEnabledException) { }
            }

            if (Preferences.Get("compass", true))
            {
                if (!(Xamarin.Essentials.Compass.IsMonitoring))
                {
                    try
                    {
                        Xamarin.Essentials.Compass.Start(_speed);
                    }
                    catch (FeatureNotEnabledException) { }
                }
            }
            else
            {
                try
                {
                    Xamarin.Essentials.Compass.Stop();
                }
                catch (FeatureNotEnabledException) { }
            }

            if (Preferences.Get("gyroscope", true))
            {
                if (!(Xamarin.Essentials.Gyroscope.IsMonitoring))
                {
                    try
                    {
                        Xamarin.Essentials.Gyroscope.Start(_speed);
                    }
                    catch (FeatureNotEnabledException) { }
                }
            }
            else
            {
                try
                {
                    Xamarin.Essentials.Gyroscope.Stop();
                }
                catch (FeatureNotEnabledException) { }
            }

            if (Preferences.Get("magnetometer", true))
            {
                if (Xamarin.Essentials.Magnetometer.IsMonitoring) return;
                try
                {
                    Xamarin.Essentials.Magnetometer.Start(_speed);
                }
                catch (FeatureNotEnabledException) { }
            }
            else
            {
                try
                {
                    Xamarin.Essentials.Magnetometer.Stop();
                }
                catch (FeatureNotEnabledException) { }
            }
        }

        public Sensor()
        {
            Accelerometer = new Sensors.Accelerometer();
            Barometer = new Sensors.Barometer();
            Compass = new Sensors.Compass();
            Gps = new Sensors.Gps();
            Gyroscope = new Sensors.Gyroscope();
            Magnetometer = new Sensors.Magnetometer();

            Init();

            Xamarin.Essentials.Accelerometer.ReadingChanged += Accelerometer.Reading_Changed;
            Xamarin.Essentials.Barometer.ReadingChanged += Barometer.Reading_Changed;
            Xamarin.Essentials.Compass.ReadingChanged += Compass.Reading_Changed;
            Xamarin.Essentials.Gyroscope.ReadingChanged += Gyroscope.Reading_Changed;
            Xamarin.Essentials.Magnetometer.ReadingChanged += Magnetometer.Reading_Changed;

            UpdateGps();
        }

        /// <summary>
        /// Updates the GPS each 10 seconds in the background
        /// </summary>
        public async void UpdateGps()
        {
            while (true)
            {
                await Gps.GetLocationAsync();

                await Task.Delay(10000);
            }
        }
    }
}
