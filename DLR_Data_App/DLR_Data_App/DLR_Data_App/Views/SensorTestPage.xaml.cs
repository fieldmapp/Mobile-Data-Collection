using System;

using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SensorTestPage
	{
    SensorSpeed _speed = SensorSpeed.UI;
    private readonly Services.Sensors.Accelerometer _accelerometer;
    private readonly Services.Sensors.Barometer _barometer;
    private readonly Services.Sensors.Compass _compass;
    private readonly Services.Sensors.Gps _gps;
    private readonly Services.Sensors.Gyroscope _gyroscope;
    private readonly Services.Sensors.Magnetometer _magnetometer;

    /**
     * Check for preference changes before opening the page
     */
    protected override async void OnAppearing()
    {
      if (Preferences.Get("accelerometer", true))
      {
        if(!(Accelerometer.IsMonitoring))
        {
          Accelerometer.Start(_speed);
          GridAccelerometer.IsEnabled = true;
        }
      }
      else
      {
        Accelerometer.Stop();
        GridAccelerometer.IsEnabled = false;
      }

      if (Preferences.Get("barometer", true))
      {
        if(!(Barometer.IsMonitoring))
        {
          Barometer.Start(_speed);
          GridBarometer.IsEnabled = true;
        }
      }
      else
      {
        Barometer.Stop();
        GridBarometer.IsEnabled = false;
      }

      if (Preferences.Get("compass", true))
      {
        if(!(Compass.IsMonitoring))
        {
          Compass.Start(_speed);
          GridCompass.IsEnabled = true;
        }
      }
      else
      {
        Compass.Stop();
        GridCompass.IsEnabled = false;
      }

      if (Preferences.Get("gps", true))
      {
        await _gps.GetLocationAsync();
      }
      else
      {
        GridGps.IsEnabled = false;
      }

      if (Preferences.Get("gyroscope", true))
      {
        if(!(Gyroscope.IsMonitoring))
        {
          Gyroscope.Start(_speed);
          GridGyroscope.IsEnabled = true;
        }
      }
      else
      {
        Gyroscope.Stop();
        GridGyroscope.IsEnabled = false;
      }

      if (Preferences.Get("magnetometer", true))
      {
        if (Magnetometer.IsMonitoring) return;
        Magnetometer.Start(_speed);
        GridMagnetometer.IsEnabled = true;
      }
      else
      {
        Magnetometer.Stop();
        GridMagnetometer.IsEnabled = false;
      }
    }

    /**
     * 
     */
    public SensorTestPage ()
		{
      _accelerometer = new Services.Sensors.Accelerometer();
      _barometer = new Services.Sensors.Barometer();
      _compass = new Services.Sensors.Compass();
      _gps = new Services.Sensors.Gps();
      _gyroscope = new Services.Sensors.Gyroscope();
      _magnetometer = new Services.Sensors.Magnetometer();

      Accelerometer.ReadingChanged += _accelerometer.Reading_Changed;
      Accelerometer.ReadingChanged += OnAccelerometer_Change;
      Accelerometer.ReadingChanged += OnGps_Change;

      Barometer.ReadingChanged += _barometer.Reading_Changed;
      Barometer.ReadingChanged += OnBarometer_Change;

      Compass.ReadingChanged += _compass.Reading_Changed;
      Compass.ReadingChanged += OnCompass_Change;

      Gyroscope.ReadingChanged += _gyroscope.Reading_Changed;
      Gyroscope.ReadingChanged += OnGyroscope_Change;

      Magnetometer.ReadingChanged += _magnetometer.Reading_Changed;
      Magnetometer.ReadingChanged += OnMagnetometer_Change;

      InitializeComponent ();
		}

    /**
     * Reset all data
     */
    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
      _accelerometer.Reset();
      _barometer.Reset();
      _compass.Reset();
      _gyroscope.Reset();
      _magnetometer.Reset();

      // TODO: optimizing
      await _gps.GetLocationAsync();

      LblXAccelerometerCurrent.Text = _accelerometer.CurrentX.ToString("N");
      LblYAccelerometerCurrent.Text = _accelerometer.CurrentY.ToString("N");
      LblZAccelerometerCurrent.Text = _accelerometer.CurrentZ.ToString("N");

      LblXAccelerometerMax.Text = _accelerometer.MaxX.ToString("N");
      LblYAccelerometerMax.Text = _accelerometer.MaxY.ToString("N");
      LblZAccelerometerMax.Text = _accelerometer.MaxZ.ToString("N");

      LblPressureBarometerCurrent.Text = _barometer.CurrentPressure.ToString("N");
      LblPressureBarometerMax.Text = _barometer.MaxPressure.ToString("N");

      LblCompassDegrees.Text = _compass.Degrees.ToString("N");
      LblCompassDirection.Text = _compass.Direction;

      LblXGyroscopeCurrent.Text = _gyroscope.CurrentX.ToString("N");
      LblYGyroscopeCurrent.Text = _gyroscope.CurrentY.ToString("N");
      LblZGyroscopeCurrent.Text = _gyroscope.CurrentZ.ToString("N");

      LblXGyroscopeMax.Text = _gyroscope.MaxX.ToString("N");
      LblYGyroscopeMax.Text = _gyroscope.MaxY.ToString("N");
      LblZGyroscopeMax.Text = _gyroscope.MaxZ.ToString("N");

      LblXMagnetometerCurrent.Text = _magnetometer.CurrentX.ToString("N");
      LblYMagnetometerCurrent.Text = _magnetometer.CurrentY.ToString("N");
      LblZMagnetometerCurrent.Text = _magnetometer.CurrentZ.ToString("N");

      LblXMagnetometerMax.Text = _magnetometer.MaxX.ToString("N");
      LblYMagnetometerMax.Text = _magnetometer.MaxY.ToString("N");
      LblZMagnetometerMax.Text = _magnetometer.MaxZ.ToString("N");
    }

    /**
     * Updates acceleration values
     */
    public void OnAccelerometer_Change(object sender, EventArgs e)
    {
      LblXAccelerometerCurrent.Text = _accelerometer.CurrentX.ToString("N");
      LblYAccelerometerCurrent.Text = _accelerometer.CurrentY.ToString("N");
      LblZAccelerometerCurrent.Text = _accelerometer.CurrentZ.ToString("N");

      LblXAccelerometerMax.Text = _accelerometer.MaxX.ToString("N");
      LblYAccelerometerMax.Text = _accelerometer.MaxY.ToString("N");
      LblZAccelerometerMax.Text = _accelerometer.MaxZ.ToString("N");
    }

    /**
     * Updates barometer values
     */
    public void OnBarometer_Change(object sender, EventArgs e)
    {
      LblPressureBarometerCurrent.Text = _barometer.CurrentPressure.ToString("N");
      LblPressureBarometerMax.Text = _barometer.MaxPressure.ToString("N");
    }

    /**
     * Updates compass values
     */
    public void OnCompass_Change(object sender, EventArgs e)
    {
      LblCompassDegrees.Text = _compass.Degrees.ToString("N");
      LblCompassDirection.Text = _compass.Direction;
    }

    /**
     * Updates gps values
     */
    public void OnGps_Change(object sender, EventArgs e)
    {
      LblLat.Text = _gps.Latitude.ToString("N6");
      LblLon.Text = _gps.Longitude.ToString("N6");
      LblAlt.Text = _gps.Altitude.ToString("N2");
      LblStatus.Text = _gps.Message;
    }

    /**
     * Updates gyroscope values
     */
    public void OnGyroscope_Change(object sender, EventArgs e)
    {
      LblXGyroscopeCurrent.Text = _gyroscope.CurrentX.ToString("N");
      LblYGyroscopeCurrent.Text = _gyroscope.CurrentY.ToString("N");
      LblZGyroscopeCurrent.Text = _gyroscope.CurrentZ.ToString("N");

      LblXGyroscopeMax.Text = _gyroscope.MaxX.ToString("N");
      LblYGyroscopeMax.Text = _gyroscope.MaxY.ToString("N");
      LblZGyroscopeMax.Text = _gyroscope.MaxZ.ToString("N");
    }

    /**
     * Updates magnetometer values
     */
    public void OnMagnetometer_Change(object sender, EventArgs e)
    {
      LblXMagnetometerCurrent.Text = _magnetometer.CurrentX.ToString("N");
      LblYMagnetometerCurrent.Text = _magnetometer.CurrentY.ToString("N");
      LblZMagnetometerCurrent.Text = _magnetometer.CurrentZ.ToString("N");

      LblXMagnetometerMax.Text = _magnetometer.MaxX.ToString("N");
      LblYMagnetometerMax.Text = _magnetometer.MaxY.ToString("N");
      LblZMagnetometerMax.Text = _magnetometer.MaxZ.ToString("N");
    }
  }
}