using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DLR_Data_App.Services;
using DLR_Data_App.Services.Sensors;
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

            _sensor = Sensor.Instance;
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

        private void OrientationSensor_ReadingChanged(object sender, Xamarin.Essentials.OrientationSensorChangedEventArgs e)
        {
            var eulerOrientation = _sensor.OrientationSensor.Orientation.ToEulerAngles();
            LblXOrientationCurrent.Text = eulerOrientation.X.ToDegrees().ToString("N");
            LblYOrientationCurrent.Text = eulerOrientation.Y.ToDegrees().ToString("N");
            LblZOrientationCurrent.Text = eulerOrientation.Z.ToDegrees().ToString("N");
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

            Velocity = 0;
            MovementSum = 0;
            MovementZ.Text = "0";
        }

        /// <summary>
        /// Updates acceleration values
        /// </summary>
        public void OnAccelerometer_Change(object sender, EventArgs e)
        {
            var partsOfOneSecond = MovementStopWatch.ElapsedMilliseconds / 1000d;
            MovementStopWatch.Restart();

            LblXAccelerometerCurrent.Text = _sensor.Accelerometer.CurrentX.ToString("N");
            LblYAccelerometerCurrent.Text = _sensor.Accelerometer.CurrentY.ToString("N");
            LblZAccelerometerCurrent.Text = _sensor.Accelerometer.CurrentZ.ToString("N");

            LblXAccelerometerMax.Text = _sensor.Accelerometer.MaxX.ToString("N");
            LblYAccelerometerMax.Text = _sensor.Accelerometer.MaxY.ToString("N");
            LblZAccelerometerMax.Text = _sensor.Accelerometer.MaxZ.ToString("N");

            if (InitStep < InitEnd)
            {
                InitSteps[InitStep] = _sensor.Accelerometer.CurrentZ;
                InitStep++;
            }
            else if (InitStep == InitEnd)
            {
                ConstantAcceleration = InitSteps.Average();
                AccelerationStandardVariance = Math.Abs(InitSteps.Average(s => s - ConstantAcceleration));
                InitStep++;
            }
            else
            {
                var velocityChange = _sensor.Accelerometer.CurrentZ - ConstantAcceleration;
                if (Math.Abs(velocityChange) > AccelerationStandardVariance * 2)
                {
                    Velocity += velocityChange * G * partsOfOneSecond;

                    MovementSum += Velocity * partsOfOneSecond;
                    MovementZ.Text = MovementSum.ToString("N");
                }
            }
        }

        Stopwatch MovementStopWatch = Stopwatch.StartNew();
        double Velocity = 0;
        double MovementSum = 0;
        const double G = 9.80665;
        double[] InitSteps = new double[InitEnd];
        double ConstantAcceleration;
        double AccelerationStandardVariance;
        int InitStep = 0;
        const int InitEnd = 1000;

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

        /// <summary>
        /// Updates gyroscope values.
        /// </summary>
        public void OnGyroscope_Change(object sender, EventArgs e)
        {
            LblXGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentX.ToString("N");
            LblYGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentY.ToString("N");
            LblZGyroscopeCurrent.Text = _sensor.Gyroscope.CurrentZ.ToString("N");

            LblXGyroscopeMax.Text = _sensor.Gyroscope.MaxX.ToString("N");
            LblYGyroscopeMax.Text = _sensor.Gyroscope.MaxY.ToString("N");
            LblZGyroscopeMax.Text = _sensor.Gyroscope.MaxZ.ToString("N");
        }

        /// <summary>
        /// Updates magnetometer values.
        /// </summary>
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