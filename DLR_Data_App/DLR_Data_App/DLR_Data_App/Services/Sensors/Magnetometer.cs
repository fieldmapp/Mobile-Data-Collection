using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Essentials;

namespace DLR_Data_App.Services.Sensors
{
  class Magnetometer
  {
    public float current_x { get; set; }
    public float current_y { get; set; }
    public float current_z { get; set; }
    public float max_x { get; set; }
    public float max_y { get; set; }
    public float max_z { get; set; }

    public Magnetometer()
    {
      this.Reset();
    }


    /**
     * Collect values after each change
     */
    public void Reading_Changed(object sender, MagnetometerChangedEventArgs e)
    {
      var data = e.Reading;

      current_x = data.MagneticField.X;
      current_y = data.MagneticField.Y;
      current_z = data.MagneticField.Z;

      if (Math.Abs(current_x) > max_x)
      {
        max_x = Math.Abs(current_x);
      }

      if (Math.Abs(current_y) > max_y)
      {
        max_y = Math.Abs(current_y);
      }

      if (Math.Abs(current_z) > max_z)
      {
        max_z = Math.Abs(current_z);
      }
    }

    /**
     * Resets values
     */
    public void Reset()
    {
      current_x = 0.0F;
      current_y = 0.0F;
      current_z = 0.0F;
      max_x = 0.0F;
      max_y = 0.0F;
      max_z = 0.0F;
    }
  }
}
