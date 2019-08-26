using System.Threading.Tasks;
using DLR_Data_App.Services.Sensors;
using Xamarin.Essentials;

namespace DLR_Data_App.Services
{
  public class Sensor
  {
    SensorSpeed _speed = SensorSpeed.UI;

    // Return GPS data
    public static async Task<Gps> GetGps()
    {
      var gps = new Gps();

      if (Preferences.Get("gps", true))
      {
        await gps.GetLocationAsync();
      }

      return gps;
    }
  }
}
