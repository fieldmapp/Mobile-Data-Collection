using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DLR_Data_App.Services.Sensors
{
    /**
     * This class handles the localization 
     */
    public class Gps
    {
        public event EventHandler<GpsEventArgs> StatusChanged;

        public double Altitude { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Message { get; set; }

        /**
         * Constructor initializes all variables
         */
        public Gps()
        {
            Altitude = 0.0;
            Latitude = 0.0;
            Longitude = 0.0;
            Message = "";
        }

        /**
         * EventHandler for handling changes in latitude, longitude and altitude
         * @param e GpsEventArgs
         */
        protected virtual void OnStatusChanged(GpsEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        /**
         * Get location
         */
        public async Task GetLocationAsync()
        {
            if (Preferences.Get("gps", true))
            {
                try
                {
                    Message = "Searching Satellites";
                    var request = new GeolocationRequest(GeolocationAccuracy.Default);
                    var location = await Geolocation.GetLocationAsync(request);

                    if (location != null)
                    {
                        //Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                        Message = "Got location";
                        Latitude = location.Latitude;
                        Longitude = location.Longitude;

                        Altitude = location.Altitude ?? 0.0;
                    }
                    else
                    {
                        Message = "Unable to get location";
                    }
                }
                catch (FeatureNotSupportedException)
                {
                    // Handle not supported on device exception
                    Message = "Function not supported";
                }
                catch (FeatureNotEnabledException)
                {
                    // Handle not enabled on device exception
                    Message = "Function not enabled";
                }
                catch (PermissionException)
                {
                    // Handle permission exception
                    Message = "Permission error";
                }
                catch (Exception)
                {
                    // Unable to get location
                    Message = "Unknown error";
                }

                OnStatusChanged(new GpsEventArgs(Message, Latitude, Longitude, Altitude));
            }
        }
    }

    public class GpsEventArgs : EventArgs
    {
        /**
         * Class for transporting data at events
         * @param message Status message
         * @param latitude Latitude coordinate
         * @param longitude Longitude coordinate
         * @param altitude Altitude
         */
        public GpsEventArgs(string message, double latitude, double longitude, double altitude)
        {
            Message = message;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        public string Message { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Altitude { get; set; }
    }
}
