using Xamarin.Essentials;

namespace DLR_Data_App.Services.Sensors
{
  /**
   * This class handles the barometer
   */
  public class Barometer
  {
    public double CurrentPressure { get; set; }
    public double MaxPressure { get; set; }

    public Barometer()
    {
      Reset();
    }

    /**
     * EventHandler which collects values after each change
     */
    public void Reading_Changed(object sender, BarometerChangedEventArgs e)
    {
      var data = e.Reading;

      CurrentPressure = data.PressureInHectopascals;

      if(CurrentPressure > MaxPressure)
      {
        MaxPressure = CurrentPressure;
      }
    }

    /**
     * Resets values
     */
    public void Reset()
    {
      CurrentPressure = 0.0;
      MaxPressure = 0.0;
    }
  }
}
