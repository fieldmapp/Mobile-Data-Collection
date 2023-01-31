using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DlrDataApp.Modules.Base.Shared.Services.Sensors
{
    public class Gps
    {
        public event EventHandler<GpsEventArgs> StatusChanged;

        public double Altitude { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
        
        public double Accuracy { get; set; }

        public string Message { get; set; }
        
        public Gps()
        {
            Altitude = 0.0;
            Latitude = 0.0;
            Accuracy = -1;
            Longitude = 0.0;
            Message = "";
        }

        /// <summary>
        /// EventHandler for handling changes in latitude, longitude and altitude
        /// </summary>
        protected virtual void OnStatusChanged(GpsEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Gets location.
        /// </summary>
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
                        Accuracy = location.Accuracy ?? -1;

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

                OnStatusChanged(new GpsEventArgs(Message, Latitude, Longitude, Accuracy, Altitude));
            }
        }
    }

    public class GpsEventArgs : EventArgs
    {
        /// <summary>
        /// Class for transporting data at events 
        /// </summary>
        /// <param name="message">Status message</param>
        /// <param name="latitude">Latitude coordinate</param>
        /// <param name="longitude">Longitude coordinate</param>
        /// <param name="altitude">Altitude</param>
        public GpsEventArgs(string message, double latitude, double longitude, double accuracy, double altitude)
        {
            Message = message;
            Latitude = latitude;
            Longitude = longitude;
            Accuracy = accuracy;
            Altitude = altitude;
        }

        public string Message { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Accuracy { get; set; }

        public double Altitude { get; set; }
    }
}
