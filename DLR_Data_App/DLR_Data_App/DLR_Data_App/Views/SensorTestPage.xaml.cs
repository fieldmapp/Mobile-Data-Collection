using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SensorTestPage : ContentPage
	{
    SensorSpeed speed = SensorSpeed.UI;
    Services.Sensors.Accelerometer accelerometer;
    Services.Sensors.Barometer barometer;
    Services.Sensors.Compass compass;
    Services.Sensors.GPS gps;
    Services.Sensors.Gyroscope gyroscope;
    Services.Sensors.Magnetometer magnetometer;

    /**
     * Check for preference changes before opening the page
     */
    protected async override void OnAppearing()
    {
      if (Preferences.Get("accelerometer", true))
      {
        if(!(Accelerometer.IsMonitoring))
        {
          Accelerometer.Start(speed);
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
          Barometer.Start(speed);
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
          Compass.Start(speed);
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
        await gps.getLocationAsync();
      }
      else
      {
        GridGps.IsEnabled = false;
      }

      if (Preferences.Get("gyroscope", true))
      {
        if(!(Gyroscope.IsMonitoring))
        {
          Gyroscope.Start(speed);
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
        if(!(Magnetometer.IsMonitoring))
        {
          Magnetometer.Start(speed);
          GridMagnetometer.IsEnabled = true;
        }
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
      accelerometer = new Services.Sensors.Accelerometer();
      barometer = new Services.Sensors.Barometer();
      compass = new Services.Sensors.Compass();
      gps = new Services.Sensors.GPS();
      gyroscope = new Services.Sensors.Gyroscope();
      magnetometer = new Services.Sensors.Magnetometer();

      Accelerometer.ReadingChanged += accelerometer.Reading_Changed;
      Accelerometer.ReadingChanged += onAccelerometer_Change;
      Accelerometer.ReadingChanged += onGps_Change;

      Barometer.ReadingChanged += barometer.Reading_Changed;
      Barometer.ReadingChanged += onBarometer_Change;

      Compass.ReadingChanged += compass.Reading_Changed;
      Compass.ReadingChanged += onCompass_Change;

      Gyroscope.ReadingChanged += gyroscope.Reading_Changed;
      Gyroscope.ReadingChanged += onGyroscope_Change;

      Magnetometer.ReadingChanged += magnetometer.Reading_Changed;
      Magnetometer.ReadingChanged += onMagnetometer_Change;

      InitializeComponent ();
		}

    /**
     * Reset all data
     */
    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
      accelerometer.Reset();
      barometer.Reset();
      compass.Reset();
      gyroscope.Reset();
      magnetometer.Reset();

      // TODO: optimizing
      await gps.getLocationAsync();

      lbl_x_accelerometer_current.Text = accelerometer.current_x.ToString("N");
      lbl_y_accelerometer_current.Text = accelerometer.current_y.ToString("N");
      lbl_z_accelerometer_current.Text = accelerometer.current_z.ToString("N");

      lbl_x_accelerometer_max.Text = accelerometer.max_x.ToString("N");
      lbl_y_accelerometer_max.Text = accelerometer.max_y.ToString("N");
      lbl_z_accelerometer_max.Text = accelerometer.max_z.ToString("N");

      lbl_pressure_barometer_current.Text = barometer.current_pressure.ToString("N");
      lbl_pressure_barometer_max.Text = barometer.max_pressure.ToString("N");

      lbl_compass_degrees.Text = compass.degrees.ToString("N");
      lbl_compass_direction.Text = compass.direction;

      lbl_x_gyroscope_current.Text = gyroscope.current_x.ToString("N");
      lbl_y_gyroscope_current.Text = gyroscope.current_y.ToString("N");
      lbl_z_gyroscope_current.Text = gyroscope.current_z.ToString("N");

      lbl_x_gyroscope_max.Text = gyroscope.max_x.ToString("N");
      lbl_y_gyroscope_max.Text = gyroscope.max_y.ToString("N");
      lbl_z_gyroscope_max.Text = gyroscope.max_z.ToString("N");

      lbl_x_magnetometer_current.Text = magnetometer.current_x.ToString("N");
      lbl_y_magnetometer_current.Text = magnetometer.current_y.ToString("N");
      lbl_z_magnetometer_current.Text = magnetometer.current_z.ToString("N");

      lbl_x_magnetometer_max.Text = magnetometer.max_x.ToString("N");
      lbl_y_magnetometer_max.Text = magnetometer.max_y.ToString("N");
      lbl_z_magnetometer_max.Text = magnetometer.max_z.ToString("N");
    }

    /**
     * Updates acceleration values
     */
    public void onAccelerometer_Change(object sender, EventArgs e)
    {
      lbl_x_accelerometer_current.Text = accelerometer.current_x.ToString("N");
      lbl_y_accelerometer_current.Text = accelerometer.current_y.ToString("N");
      lbl_z_accelerometer_current.Text = accelerometer.current_z.ToString("N");

      lbl_x_accelerometer_max.Text = accelerometer.max_x.ToString("N");
      lbl_y_accelerometer_max.Text = accelerometer.max_y.ToString("N");
      lbl_z_accelerometer_max.Text = accelerometer.max_z.ToString("N");
    }

    /**
     * Updates barometer values
     */
    public void onBarometer_Change(object sender, EventArgs e)
    {
      lbl_pressure_barometer_current.Text = barometer.current_pressure.ToString("N");
      lbl_pressure_barometer_max.Text = barometer.max_pressure.ToString("N");
    }

    /**
     * Updates compass values
     */
    public void onCompass_Change(object sender, EventArgs e)
    {
      lbl_compass_degrees.Text = compass.degrees.ToString("N");
      lbl_compass_direction.Text = compass.direction;
    }

    /**
     * Updates gps values
     */
    public void onGps_Change(object sender, EventArgs e)
    {
      lbl_lat.Text = gps.Latitude.ToString("N6");
      lbl_lon.Text = gps.Longitude.ToString("N6");
      lbl_alt.Text = gps.Altitude.ToString("N2");
      lbl_status.Text = gps.Message;
    }

    /**
     * Updates gyroscope values
     */
    public void onGyroscope_Change(object sender, EventArgs e)
    {
      lbl_x_gyroscope_current.Text = gyroscope.current_x.ToString("N");
      lbl_y_gyroscope_current.Text = gyroscope.current_y.ToString("N");
      lbl_z_gyroscope_current.Text = gyroscope.current_z.ToString("N");

      lbl_x_gyroscope_max.Text = gyroscope.max_x.ToString("N");
      lbl_y_gyroscope_max.Text = gyroscope.max_y.ToString("N");
      lbl_z_gyroscope_max.Text = gyroscope.max_z.ToString("N");
    }

    /**
     * Updates magnetometer values
     */
    public void onMagnetometer_Change(object sender, EventArgs e)
    {
      lbl_x_magnetometer_current.Text = magnetometer.current_x.ToString("N");
      lbl_y_magnetometer_current.Text = magnetometer.current_y.ToString("N");
      lbl_z_magnetometer_current.Text = magnetometer.current_z.ToString("N");

      lbl_x_magnetometer_max.Text = magnetometer.max_x.ToString("N");
      lbl_y_magnetometer_max.Text = magnetometer.max_y.ToString("N");
      lbl_z_magnetometer_max.Text = magnetometer.max_z.ToString("N");
    }
  }
}