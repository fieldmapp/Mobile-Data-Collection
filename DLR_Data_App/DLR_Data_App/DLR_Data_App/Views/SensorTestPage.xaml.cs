using System;
using DLR_Data_App.Services;
using DlrDataApp.Modules.Base.Shared.Services;
using DlrDataApp.Modules.Base.Shared.Services.Sensors;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorTestPage
    {
        private readonly Sensor _sensor;
        
        public SensorTestPage()
        {
            InitializeComponent();

            _sensor = (App.Current as App).Sensor;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _sensor.Accelerometer.ReadingChanged += OnAccelerometer_Change;
            _sensor.Gps.StatusChanged += OnGps_Change;
            _sensor.Barometer.ReadingChanged += OnBarometer_Change;
            _sensor.Compass.ReadingChanged += OnCompass_Change;
            _sensor.Gyroscope.ReadingChanged += OnGyroscope_Change;
            _sensor.Magnetometer.ReadingChanged += OnMagnetometer_Change;
            _sensor.OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
        }

        int DisplayOrientationSensorCounter;

        private void OrientationSensor_ReadingChanged(object sender, Xamarin.Essentials.OrientationSensorChangedEventArgs e)
        {
            DisplayOrientationSensorCounter++;
            if (DisplayOrientationSensorCounter > 10)
            {
                DisplayOrientationSensorCounter = 0;
                var eulerOrientation = _sensor.OrientationSensor.Orientation.ToEulerAngles();
                Device.BeginInvokeOnMainThread(() =>
                {
                    LblXOrientationCurrent.Text = eulerOrientation.X.ToDegrees().ToString("N");
                    LblYOrientationCurrent.Text = eulerOrientation.Y.ToDegrees().ToString("N");
                    LblZOrientationCurrent.Text = eulerOrientation.Z.ToDegrees().ToString("N");
                });
            }
        }

        protected override void OnDisappearing()
        {
            _sensor.Accelerometer.ReadingChanged -= OnAccelerometer_Change;
            _sensor.Gps.StatusChanged -= OnGps_Change;
            _sensor.Barometer.ReadingChanged -= OnBarometer_Change;
            _sensor.Compass.ReadingChanged -= OnCompass_Change;
            _sensor.Gyroscope.ReadingChanged -= OnGyroscope_Change;
            _sensor.Magnetometer.ReadingChanged -= OnMagnetometer_Change;
            _sensor.OrientationSensor.ReadingChanged -= OrientationSensor_ReadingChanged;
        }

        /// <summary>
        /// Resets all data.
        /// </summary>
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            _sensor.Accelerometer.Reset();
            _sensor.Barometer.Reset();
            _sensor.Compass.Reset();
            _sensor.Gyroscope.Reset();
            _sensor.Magnetometer.Reset();
            _sensor.OrientationSensor.Reset();

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

            var eulerOrientation = _sensor.OrientationSensor.Orientation.ToEulerAngles();
            LblXOrientationCurrent.Text = eulerOrientation.X.ToDegrees().ToString("N");
            LblYOrientationCurrent.Text = eulerOrientation.Y.ToDegrees().ToString("N");
            LblZOrientationCurrent.Text = eulerOrientation.Z.ToDegrees().ToString("N");
        }

        int AccelerometerDisplayCounter;

        /// <summary>
        /// Updates acceleration values
        /// </summary>
        public void OnAccelerometer_Change(object sender, EventArgs e)
        {
            var currentX = _sensor.Accelerometer.CurrentX.ToString("N");
            var currentY = _sensor.Accelerometer.CurrentY.ToString("N");
            var currentZ = _sensor.Accelerometer.CurrentZ.ToString("N");

            var maxX = _sensor.Accelerometer.MaxX.ToString("N");
            var maxY = _sensor.Accelerometer.MaxY.ToString("N");
            var maxZ = _sensor.Accelerometer.MaxZ.ToString("N");

            AccelerometerDisplayCounter++;
            if (AccelerometerDisplayCounter > 10)
            {
                AccelerometerDisplayCounter = 0;
                Device.BeginInvokeOnMainThread(() =>
                {
                    LblXAccelerometerCurrent.Text = currentX;
                    LblYAccelerometerCurrent.Text = currentY;
                    LblZAccelerometerCurrent.Text = currentZ;

                    LblXAccelerometerMax.Text = maxX;
                    LblYAccelerometerMax.Text = maxY;
                    LblZAccelerometerMax.Text = maxZ;
                });
            }
        }

        /// <summary>
        /// Updates barometer values
        /// </summary>
        public void OnBarometer_Change(object sender, EventArgs e)
        {
            LblPressureBarometerCurrent.Text = _sensor.Barometer.CurrentPressure.ToString("N");
            LblPressureBarometerMax.Text = _sensor.Barometer.MaxPressure.ToString("N");
        }

        /// <summary>
        /// Updates compass values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnCompass_Change(object sender, EventArgs e)
        {
            LblCompassDegrees.Text = _sensor.Compass.Degrees.ToString("N");
            LblCompassDirection.Text = _sensor.Compass.Direction;
        }

        /// <summary>
        /// Updates gps values.
        /// </summary>
        public void OnGps_Change(object sender, GpsEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                LblLat.Text = e.Latitude.ToString("N6");
                LblLon.Text = e.Longitude.ToString("N6");
                LblAlt.Text = e.Altitude.ToString("N2");
                LblStatus.Text = e.Message;
            });
        }

        int GyroscopeDisplayCounter;

        /// <summary>
        /// Updates gyroscope values.
        /// </summary>
        public void OnGyroscope_Change(object sender, EventArgs e)
        {
            GyroscopeDisplayCounter++;
            if (GyroscopeDisplayCounter > 10)
            {
                GyroscopeDisplayCounter = 0;
                Device.BeginInvokeOnMainThread(() =>
                {
                    LblXGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentX.ToString("N");
                    LblYGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentY.ToString("N");
                    LblZGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentZ.ToString("N");

                    LblXGyroscopeMax.Text = _sensor.Gyroscope.MaxX.ToString("N");
                    LblYGyroscopeMax.Text = _sensor.Gyroscope.MaxY.ToString("N");
                    LblZGyroscopeMax.Text = _sensor.Gyroscope.MaxZ.ToString("N");
                });
            }
        }

        int MagnetometerDisplayCounter;
        /// <summary>
        /// Updates magnetometer values.
        /// </summary>
        public void OnMagnetometer_Change(object sender, EventArgs e)
        {
            MagnetometerDisplayCounter++;

            if (MagnetometerDisplayCounter > 10)
            {
                MagnetometerDisplayCounter = 0;
                Device.BeginInvokeOnMainThread(() =>
                {
                    LblXMagnetometerCurrent.Text = _sensor.Magnetometer.CurrentX.ToString("N");
                    LblYMagnetometerCurrent.Text = _sensor.Magnetometer.CurrentY.ToString("N");
                    LblZMagnetometerCurrent.Text = _sensor.Magnetometer.CurrentZ.ToString("N");

                    LblXMagnetometerMax.Text = _sensor.Magnetometer.MaxX.ToString("N");
                    LblYMagnetometerMax.Text = _sensor.Magnetometer.MaxY.ToString("N");
                    LblZMagnetometerMax.Text = _sensor.Magnetometer.MaxZ.ToString("N");
                });
            }
        }
    }
}