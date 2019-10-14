using Xamarin.Essentials;

namespace DLR_Data_App.Services.Sensors
{
  /**
   * This class handles the compass
   */
  public class Compass
  {
    public double Degrees { get; set; }
    public string Direction { get; set; }

    public Compass()
    {
      Reset();
    }

    /**
     * Collect values after each change
     */
    public void Reading_Changed(object sender, CompassChangedEventArgs e)
    {
      var data = e.Reading;

      Degrees = data.HeadingMagneticNorth;
      CalcDirection();
    }

    /**
     * Resets values
     */
    public void Reset()
    {
      Degrees = 0.0;
      Direction = "N";
    }

    /** 
     * Generates direction from angle of degrees
     */
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
