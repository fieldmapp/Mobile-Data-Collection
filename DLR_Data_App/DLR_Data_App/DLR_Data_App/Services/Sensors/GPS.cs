﻿using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DLR_Data_App.Services.Sensors
{
  public class Gps
  {
    public double Altitude { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Message { get; set; }

    public Gps()
    {
      Altitude = 0.0;
      Latitude = 0.0;
      Longitude = 0.0;
      Message = "";
    }

    public async Task GetLocationAsync()
    {
      try
      {
        Message = "Searching Satellites";
        var request = new GeolocationRequest(GeolocationAccuracy.Best);
        var location = await Geolocation.GetLocationAsync(request);

        if (location != null)
        {
          //Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
          Message = "Got location";
          Latitude = location.Latitude;
          Longitude = location.Longitude;

          Altitude = 0.0;
          if (location.Altitude != null)
          {
            Altitude = location.Altitude.Value;
          }
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
    }
  }
}
