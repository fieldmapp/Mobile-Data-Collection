using System;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorTestPage
    {
        private readonly Sensor _sensor;

        /**
         * This class prints all available sensor data
         */
        public SensorTestPage()
        {
            _sensor = new Sensor();

            Accelerometer.ReadingChanged += _sensor.Accelerometer.Reading_Changed;
            Accelerometer.ReadingChanged += OnAccelerometer_Change;

            _sensor.Gps.StatusChanged += OnGps_Change;

            Barometer.ReadingChanged += _sensor.Barometer.Reading_Changed;
            Barometer.ReadingChanged += OnBarometer_Change;

            Compass.ReadingChanged += _sensor.Compass.Reading_Changed;
            Compass.ReadingChanged += OnCompass_Change;

            Gyroscope.ReadingChanged += _sensor.Gyroscope.Reading_Changed;
            Gyroscope.ReadingChanged += OnGyroscope_Change;

            Magnetometer.ReadingChanged += _sensor.Magnetometer.Reading_Changed;
            Magnetometer.ReadingChanged += OnMagnetometer_Change;

            InitializeComponent();
        }

        /**
         * Override hardware back button on Android devices to return to project list
         */
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();
                if ((Application.Current as App).CurrentPage is MainPage mainPage)
                    await mainPage.NavigateFromMenu(MenuItemType.Projects);
            });

            return true;
        }

        /**
         * Reset all data
         */
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            _sensor.Accelerometer.Reset();
            _sensor.Barometer.Reset();
            _sensor.Compass.Reset();
            _sensor.Gyroscope.Reset();
            _sensor.Magnetometer.Reset();

            LblXAccelerometerCurrent.Text = _sensor.Accelerometer.CurrentX.ToString("N");
            LblYAccelerometerCurrent.Text = _sensor.Accelerometer.CurrentY.ToString("N");
            LblZAccelerometerCurrent.Text = _sensor.Accelerometer.CurrentZ.ToString("N");

            LblXAccelerometerMax.Text = _sensor.Accelerometer.MaxX.ToString("N");
            LblYAccelerometerMax.Text = _sensor.Accelerometer.MaxY.ToString("N");
            LblZAccelerometerMax.Text = _sensor.Accelerometer.MaxZ.ToString("N");

            LblPressureBarometerCurrent.Text = _sensor.Barometer.CurrentPressure.ToString("N");
            LblPressureBarometerMax.Text = _sensor.Barometer.MaxPressure.ToString("N");

            LblCompassDegrees.Text = _sensor.Compass.Degrees.ToString("N");
            LblCompassDirection.Text = _sensor.Compass.Direction;

            LblXGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentX.ToString("N");
            LblYGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentY.ToString("N");
            LblZGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentZ.ToString("N");

            LblXGyroscopeMax.Text = _sensor.Gyroscope.MaxX.ToString("N");
            LblYGyroscopeMax.Text = _sensor.Gyroscope.MaxY.ToString("N");
            LblZGyroscopeMax.Text = _sensor.Gyroscope.MaxZ.ToString("N");

            LblXMagnetometerCurrent.Text = _sensor.Magnetometer.CurrentX.ToString("N");
            LblYMagnetometerCurrent.Text = _sensor.Magnetometer.CurrentY.ToString("N");
            LblZMagnetometerCurrent.Text = _sensor.Magnetometer.CurrentZ.ToString("N");

            LblXMagnetometerMax.Text = _sensor.Magnetometer.MaxX.ToString("N");
            LblYMagnetometerMax.Text = _sensor.Magnetometer.MaxY.ToString("N");
            LblZMagnetometerMax.Text = _sensor.Magnetometer.MaxZ.ToString("N");
        }

        /**
         * Updates acceleration values
         */
        public void OnAccelerometer_Change(object sender, EventArgs e)
        {
            LblXAccelerometerCurrent.Text = _sensor.Accelerometer.CurrentX.ToString("N");
            LblYAccelerometerCurrent.Text = _sensor.Accelerometer.CurrentY.ToString("N");
            LblZAccelerometerCurrent.Text = _sensor.Accelerometer.CurrentZ.ToString("N");

            LblXAccelerometerMax.Text = _sensor.Accelerometer.MaxX.ToString("N");
            LblYAccelerometerMax.Text = _sensor.Accelerometer.MaxY.ToString("N");
            LblZAccelerometerMax.Text = _sensor.Accelerometer.MaxZ.ToString("N");
        }

        /**
         * Updates barometer values
         */
        public void OnBarometer_Change(object sender, EventArgs e)
        {
            LblPressureBarometerCurrent.Text = _sensor.Barometer.CurrentPressure.ToString("N");
            LblPressureBarometerMax.Text = _sensor.Barometer.MaxPressure.ToString("N");
        }

        /**
         * Updates compass values
         */
        public void OnCompass_Change(object sender, EventArgs e)
        {
            LblCompassDegrees.Text = _sensor.Compass.Degrees.ToString("N");
            LblCompassDirection.Text = _sensor.Compass.Direction;
        }

        /**
         * Updates gps values
         */
        public void OnGps_Change(object sender, EventArgs e)
        {
            LblLat.Text = _sensor.Gps.Latitude.ToString("N6");
            LblLon.Text = _sensor.Gps.Longitude.ToString("N6");
            LblAlt.Text = _sensor.Gps.Altitude.ToString("N2");
            LblStatus.Text = _sensor.Gps.Message;

            //sensor._gps.GetLocationAsync();
        }

        /**
         * Updates gyroscope values
         */
        public void OnGyroscope_Change(object sender, EventArgs e)
        {
            LblXGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentX.ToString("N");
            LblYGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentY.ToString("N");
            LblZGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentZ.ToString("N");

            LblXGyroscopeMax.Text = _sensor.Gyroscope.MaxX.ToString("N");
            LblYGyroscopeMax.Text = _sensor.Gyroscope.MaxY.ToString("N");
            LblZGyroscopeMax.Text = _sensor.Gyroscope.MaxZ.ToString("N");
        }

        /**
         * Updates magnetometer values
         */
        public void OnMagnetometer_Change(object sender, EventArgs e)
        {
            LblXMagnetometerCurrent.Text = _sensor.Magnetometer.CurrentX.ToString("N");
            LblYMagnetometerCurrent.Text = _sensor.Magnetometer.CurrentY.ToString("N");
            LblZMagnetometerCurrent.Text = _sensor.Magnetometer.CurrentZ.ToString("N");

            LblXMagnetometerMax.Text = _sensor.Magnetometer.MaxX.ToString("N");
            LblYMagnetometerMax.Text = _sensor.Magnetometer.MaxY.ToString("N");
            LblZMagnetometerMax.Text = _sensor.Magnetometer.MaxZ.ToString("N");
        }
    }
}