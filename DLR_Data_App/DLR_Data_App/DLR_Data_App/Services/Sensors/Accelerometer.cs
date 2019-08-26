using System;

using Xamarin.Essentials;


namespace DLR_Data_App.Services.Sensors
{
  public class Accelerometer
  {
    public float CurrentX { get; set; }
    public float CurrentY { get; set; }
    public float CurrentZ { get; set; }
    public float MaxX { get; set; }
    public float MaxY { get; set; }
    public float MaxZ { get; set; }

    public Accelerometer()
    {
      Reset();
    }
    
    /**
     * Collect values after each change
     */
    public void Reading_Changed(object sender, AccelerometerChangedEventArgs e)
    {
      var data = e.Reading;

      CurrentX = data.Acceleration.X;
      CurrentY = data.Acceleration.Y;
      CurrentZ = data.Acceleration.Z;

      if(Math.Abs(CurrentX) > MaxX)
      {
        MaxX = Math.Abs(CurrentX);
      }

      if (Math.Abs(CurrentY) > MaxY)
      {
        MaxY = Math.Abs(CurrentY);
      }

      if (Math.Abs(CurrentZ) > MaxZ)
      {
        MaxZ = Math.Abs(CurrentZ);
      }
    }

    /**
     * Resets values
     */
    public void Reset()
    {
      CurrentX = 0.0F;
      CurrentY = 0.0F;
      CurrentZ = 0.0F;
      MaxX = 0.0F;
      MaxY = 0.0F;
      MaxZ = 0.0F;
    }
  }
}
