using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SensorListPage : ContentPage
	{
		public SensorListPage ()
		{
			InitializeComponent ();
		}

    /**
     */
    private void Init()
    {
      switch_accelerometer.IsToggled = Preferences.Get("accelerometer", true);
      switch_barometer.IsToggled = Preferences.Get("barometer", true);
      switch_compass.IsToggled = Preferences.Get("compass", true);
      switch_gps.IsToggled = Preferences.Get("gps", true);
      switch_gyroscope.IsToggled = Preferences.Get("gyroscope", true);
      switch_magnetometer.IsToggled = Preferences.Get("magnetometer", true);
    }

    private void Switch_accelerometer_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("accelerometer", switch_accelerometer.IsToggled);
    }

    private void Switch_barometer_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("barometer", switch_barometer.IsToggled);
    }

    private void Switch_compass_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("compass", switch_compass.IsToggled);
    }

    private void Switch_gps_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("gps", switch_gps.IsToggled);
    }

    private void Switch_gyroscope_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("gyroscope", switch_gyroscope.IsToggled);
    }

    private void Switch_magnetometer_Toggled(object sender, ToggledEventArgs e)
    {
      Preferences.Set("magnetometer", switch_magnetometer.IsToggled);
    }
	}
}