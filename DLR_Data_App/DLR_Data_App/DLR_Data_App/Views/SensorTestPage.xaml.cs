﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

        int display2;

        private void OrientationSensor_ReadingChanged(object sender, Xamarin.Essentials.OrientationSensorChangedEventArgs e)
        {
            display2++;
            if (display2 > 10)
            {
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

            Velocity = new Vector3();
            MovedDistance = new Vector3();
            InitStep = 0;
            MovementZ.Text = "0";
        }

        /// <summary>
        /// Updates acceleration values
        /// </summary>
        public void OnAccelerometer_Change(object sender, EventArgs e)
        {
            var partsOfOneSecond = MovementStopWatch.ElapsedMilliseconds / 1000f;
            MovementStopWatch.Restart();

            var currentX = _sensor.Accelerometer.CurrentX.ToString("N");
            var currentY = _sensor.Accelerometer.CurrentY.ToString("N");
            var currentZ = _sensor.Accelerometer.CurrentZ.ToString("N");

            var maxX = _sensor.Accelerometer.MaxX.ToString("N");
            var maxY = _sensor.Accelerometer.MaxY.ToString("N");
            var maxZ = _sensor.Accelerometer.MaxZ.ToString("N");

            Device.BeginInvokeOnMainThread(() =>
            {
                LblXAccelerometerCurrent.Text = currentX;
                LblYAccelerometerCurrent.Text = currentY;
                LblZAccelerometerCurrent.Text = currentZ;

                LblXAccelerometerMax.Text = maxX;
                LblYAccelerometerMax.Text = maxY;
                LblZAccelerometerMax.Text = maxZ;
            });

            if (InitStep < InitEnd)
            {
                
                InitSteps[InitStep] = Vector3.Transform(_sensor.Accelerometer.Current, _sensor.OrientationSensor.Orientation);
                InitStep++;
            }
            else if (InitStep == InitEnd)
            {
                var sampleSum = Vector3.Zero;
                for (int i = 0; i < InitEnd; i++)
                {
                    sampleSum += InitSteps[i];
                }

                ConstantAcceleration = sampleSum / InitEnd;
                var sampleDerivationSum = Vector3.Zero;
                for (int i = 0; i < InitEnd; i++)
                {
                    sampleDerivationSum += (InitSteps[i] - ConstantAcceleration).Abs();
                }
                AccelerationStandardVariance = sampleDerivationSum.PointwiseDoubleOperation(Math.Sqrt) / InitEnd;
                InitStep++;
            }
            else
            {
                var velocityChange = Vector3.Transform(_sensor.Accelerometer.Current, _sensor.OrientationSensor.Orientation) - ConstantAcceleration;
                float xChange = velocityChange.X, 
                    yChange = velocityChange.Y, 
                    zChange = velocityChange.Z;
                if (Math.Abs(xChange) < AccelerationStandardVariance.X)
                    xChange = 0;
                if (Math.Abs(yChange) < AccelerationStandardVariance.Y)
                    yChange = 0;
                if (Math.Abs(zChange) < AccelerationStandardVariance.Z)
                    zChange = 0;
                velocityChange = new Vector3(xChange, yChange, zChange);

                Velocity += velocityChange * G * partsOfOneSecond;

                MovedDistance += Velocity * partsOfOneSecond;

                display++;
                if (display > 10)
                {
                    display = 0;

                    var localMovedXDistance = MovedDistance.X.ToString("N");
                    var localMovedYDistance = MovedDistance.Y.ToString("N");
                    var localMovedZDistance = MovedDistance.Z.ToString("N");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MovementX.Text = localMovedXDistance;
                        MovementY.Text = localMovedYDistance;
                        MovementZ.Text = localMovedZDistance;
                    });
                }
            }
        }

        int display = 0;

        Stopwatch MovementStopWatch = Stopwatch.StartNew();
        Vector3 Velocity;
        Vector3 MovedDistance;
        const float G = 9.80665f;
        Vector3[] InitSteps = new Vector3[InitEnd];
        Vector3 ConstantAcceleration;
        Vector3 AccelerationStandardVariance;
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