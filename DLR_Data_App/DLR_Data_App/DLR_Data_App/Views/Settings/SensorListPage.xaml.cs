using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorListPage
    {
        public SensorListPage()
        {
            InitializeComponent();
            Init();
        }
        
        private void Init()
        {
            SwitchAccelerometer.IsToggled = Preferences.Get("accelerometer", true);
            SwitchBarometer.IsToggled = Preferences.Get("barometer", true);
            SwitchCompass.IsToggled = Preferences.Get("compass", true);
            SwitchGps.IsToggled = Preferences.Get("gps", true);
            SwitchGyroscope.IsToggled = Preferences.Get("gyroscope", true);
            SwitchMagnetometer.IsToggled = Preferences.Get("magnetometer", true);
            SwitchOrientationSensor.IsToggled = Preferences.Get("orientationsensor", true);
        }

        private void Switch_accelerometer_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("accelerometer", SwitchAccelerometer.IsToggled);
        }

        private void Switch_barometer_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("barometer", SwitchBarometer.IsToggled);
        }

        private void Switch_compass_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("compass", SwitchCompass.IsToggled);
        }

        private void Switch_gps_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("gps", SwitchGps.IsToggled);
        }

        private void Switch_gyroscope_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("gyroscope", SwitchGyroscope.IsToggled);
        }

        private void Switch_magnetometer_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("magnetometer", SwitchMagnetometer.IsToggled);
        }

        private void SwitchOrientationSensor_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("orientationsensor", SwitchOrientationSensor.IsToggled);
        }
    }
}