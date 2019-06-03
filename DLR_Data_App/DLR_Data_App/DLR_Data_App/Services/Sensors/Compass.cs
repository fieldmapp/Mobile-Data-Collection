using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Essentials;

namespace DLR_Data_App.Services.Sensors
{
  class Compass
  {
    public double degrees { get; set; }
    public String direction { get; set; }

    public Compass()
    {
      this.Reset();
    }

    /**
     * Collect values after each change
     */
    public void Reading_Changed(object sender, CompassChangedEventArgs e)
    {
      var data = e.Reading;

      degrees = data.HeadingMagneticNorth;
      calcDirection();
    }

    /**
     * Resets values
     */
    public void Reset()
    {
      degrees = 0.0;
      direction = "N";
    }

    /** 
     * Generates direction from angle of degrees
     */
    private void calcDirection()
    {
      if (degrees > 337.5 || degrees < 22.5)
      {
        direction = "N";
      }

      if (degrees >= 22.5 && degrees < 67.5)
      {
        direction = "NE";
      }

      if (degrees >= 67.5 && degrees < 112.5)
      {
        direction = "E";
      }

      if (degrees >= 112.5 && degrees < 157.5)
      {
        direction = "SE";
      }

      if (degrees >= 157.5 && degrees < 202.5)
      {
        direction = "S";
      }

      if (degrees >= 202.5 && degrees < 247.5)
      {
        direction = "SW";
      }

      if (degrees >= 247.5 && degrees < 292.5)
      {
        direction = "W";
      }

      if (degrees >= 292.5 && degrees < 337.5)
      {
        direction = "NW";
      }
    }
  }
}
