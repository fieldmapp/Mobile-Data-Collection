using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DLR_Data_App.Services
{
    /**
     * This class controls all available sensors
     */
    public class Sensor
    {
        private SensorSpeed _speed = SensorSpeed.UI;

        public Sensors.Accelerometer Accelerometer;
        public Sensors.Barometer Barometer;
        public Sensors.Compass Compass;
        public Sensors.Gps Gps;
        public Sensors.Gyroscope Gyroscope;
        public Sensors.Magnetometer Magnetometer;

        /**
         * Check for preference changes before opening the page
         */
        private void Init()
        {
            if (Preferences.Get("accelerometer", true))
            {
                if (!(Xamarin.Essentials.Accelerometer.IsMonitoring))
                {
                    Xamarin.Essentials.Accelerometer.Start(_speed);
                }
            }
            else
            {
                Xamarin.Essentials.Accelerometer.Stop();
            }

            if (Preferences.Get("barometer", true))
            {
                if (!(Xamarin.Essentials.Barometer.IsMonitoring))
                {
                    Xamarin.Essentials.Barometer.Start(_speed);
                }
            }
            else
            {
                Xamarin.Essentials.Barometer.Stop();
            }

            if (Preferences.Get("compass", true))
            {
                if (!(Xamarin.Essentials.Compass.IsMonitoring))
                {
                    Xamarin.Essentials.Compass.Start(_speed);
                }
            }
            else
            {
                Xamarin.Essentials.Compass.Stop();
            }

            if (Preferences.Get("gyroscope", true))
            {
                if (!(Xamarin.Essentials.Gyroscope.IsMonitoring))
                {
                    Xamarin.Essentials.Gyroscope.Start(_speed);
                }
            }
            else
            {
                Xamarin.Essentials.Gyroscope.Stop();
            }

            if (Preferences.Get("magnetometer", true))
            {
                if (Xamarin.Essentials.Magnetometer.IsMonitoring) return;
                Xamarin.Essentials.Magnetometer.Start(_speed);
            }
            else
            {
                Xamarin.Essentials.Magnetometer.Stop();
            }
        }

        /**
         * Constructor for initializing sensors
         */
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

        /**
         * Updating GPS each 10 seconds in the background
         */
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
