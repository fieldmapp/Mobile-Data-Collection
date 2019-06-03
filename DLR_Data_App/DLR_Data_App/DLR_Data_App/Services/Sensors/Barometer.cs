using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Essentials;

namespace DLR_Data_App.Services.Sensors
{
  class Barometer
  {
    public double current_pressure { get; set; }
    public double max_pressure { get; set; }

    public Barometer()
    {
      this.Reset();
    }

    /**
     * Collect values after each change
     */
    public void Reading_Changed(object sender, BarometerChangedEventArgs e)
    {
      var data = e.Reading;

      current_pressure = data.PressureInHectopascals;

      if(current_pressure > max_pressure)
      {
        max_pressure = current_pressure;
      }
    }

    /**
     * Resets values
     */
    public void Reset()
    {
      this.current_pressure = 0.0;
      this.max_pressure = 0.0;
    }
  }
}
