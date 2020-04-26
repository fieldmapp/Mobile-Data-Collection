using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GaitTrackingMeasuringDemoPage : ContentPage
    {
        static readonly TimeSpan MeasuringDuration = TimeSpan.FromSeconds(10);
        struct AccelerationDataPoint
        {
            public TimeSpan TimeStamp;
            public Vector3 Acceleration;

            public AccelerationDataPoint(TimeSpan timeStamp, Vector3 acceleration)
            {
                TimeStamp = timeStamp;
                Acceleration = acceleration;
            }
        }

        struct GyroscopeDataPoint
        {
            public TimeSpan TimeStamp;
            public Vector3 Rotation;

            public GyroscopeDataPoint(TimeSpan timeStamp, Vector3 rotation)
            {
                TimeStamp = timeStamp;
                Rotation = rotation;
            }
        }

        struct MagnetometerDataPoint
        {
            public TimeSpan TimeStamp;
            public Vector3 MagneticSample;

            public MagnetometerDataPoint(TimeSpan timeStamp, Vector3 magneticSample)
            {
                TimeStamp = timeStamp;
                MagneticSample = magneticSample;
            }
        }

        public struct CombinedDataPoint
        {
            public TimeSpan TimeStamp;

            public Vector3 MagneticField;
            public Vector3 GyroscopeData;
            public Vector3 Acceleration;

            public CombinedDataPoint(TimeSpan timeStamp, Vector3 magneticField, Vector3 gyroscopeData, Vector3 acceleration)
            {
                TimeStamp = timeStamp;
                MagneticField = magneticField;
                GyroscopeData = gyroscopeData;
                Acceleration = acceleration;
            }
        }

        List<AccelerationDataPoint> AccelerometerDataPoints = new List<AccelerationDataPoint>();
        List<GyroscopeDataPoint> GyroscopeDataPoints = new List<GyroscopeDataPoint>();
        List<MagnetometerDataPoint> MagnetometerDataPoints = new List<MagnetometerDataPoint>();

        Stopwatch Timekeeper = new Stopwatch();

        bool Started = false;

        public GaitTrackingMeasuringDemoPage()
        {
            InitializeComponent();
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            if (Started)
            {
                Timekeeper.Stop();
                Sensor.Instance.Gyroscope.ReadingChanged -= Gyroscope_ReadingChanged;
                Sensor.Instance.Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
                Sensor.Instance.Magnetometer.ReadingChanged -= Magnetometer_ReadingChanged;
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                StringBuilder s = new StringBuilder();
                s.AppendLine(@"Packet number, Gyroscope X(deg / s), Gyroscope Y(deg / s), Gyroscope Z(deg / s), Accelerometer X(g), Accelerometer Y(g), Accelerometer Z(g), Magnetometer X(G), Magnetometer Y(G), Magnetometer Z(G))");
                
                var text = JsonTranslator.GetJson(new { acc = AccelerometerDataPoints, gyr = GyroscopeDataPoints, mag = MagnetometerDataPoints });

                var combinedData = CombineData(AccelerometerDataPoints, GyroscopeDataPoints, MagnetometerDataPoints);
                for (int i = 0; i < combinedData.Count; i++)
                {
                    s.AppendLine($"{i},{combinedData[i].GyroscopeData.X * 180f / Math.PI},{combinedData[i].GyroscopeData.Y * 180f / Math.PI},{combinedData[i].GyroscopeData.Z * 180f / Math.PI},{combinedData[i].Acceleration.X},{combinedData[i].Acceleration.Y},{combinedData[i].Acceleration.Z},{combinedData[i].MagneticField.X / 100f},{combinedData[i].MagneticField.Y / 100f},{combinedData[i].MagneticField.Z / 100f}");
                }
                var result = s.ToString();
                CenterText.Text = "Done";
                CenterText.IsVisible = true;
                AccelerometerDataPoints = new List<AccelerationDataPoint>();
                GyroscopeDataPoints = new List<GyroscopeDataPoint>();
                MagnetometerDataPoints = new List<MagnetometerDataPoint>();
                StartButton.Text = AppResources.start;
                Started = false;
                Layout.BackgroundColor = Color.White;
                Timekeeper.Reset();
            }
            else
            {
                Started = true;
                Timekeeper.Start();
                Sensor.Instance.Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
                Sensor.Instance.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                Sensor.Instance.Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
                StartButton.IsVisible = false;
                Layout.BackgroundColor = Color.Red;
                CenterText.IsVisible = true;
                Layout.BackgroundColor = Color.Green;
                StartButton.Text = AppResources.finish;
                StartButton.IsVisible = true;
            }
        }

        private void Magnetometer_ReadingChanged(object sender, Xamarin.Essentials.MagnetometerChangedEventArgs e)
        {
            MagnetometerDataPoints.Add(new MagnetometerDataPoint(Timekeeper.Elapsed, e.Reading.MagneticField));
        }

        private void Gyroscope_ReadingChanged(object sender, Xamarin.Essentials.GyroscopeChangedEventArgs e)
        {
            GyroscopeDataPoints.Add(new GyroscopeDataPoint(Timekeeper.Elapsed, e.Reading.AngularVelocity));
        }

        private void Accelerometer_ReadingChanged(object sender, Xamarin.Essentials.AccelerometerChangedEventArgs e)
        {
            AccelerometerDataPoints.Add(new AccelerationDataPoint(Timekeeper.Elapsed, e.Reading.Acceleration));
        }

        private List<CombinedDataPoint> CombineData(List<AccelerationDataPoint> accelerations, List<GyroscopeDataPoint> orientations, List<MagnetometerDataPoint> magneticFields)
        {
            var result = new List<CombinedDataPoint>();
            for (int i = 0; i < accelerations.Count; i++)
            {
                var nearestSmallerGyroscopeIndex = -1;
                var nearestSmallerMagneticIndex = -1;
                for (int j = orientations.Count - 1; j >= 0; j--)
                {
                    if (accelerations[i].TimeStamp > orientations[j].TimeStamp)
                    {
                        nearestSmallerGyroscopeIndex = j;
                        break;
                    }
                }

                if (nearestSmallerGyroscopeIndex == -1)
                    continue;

                for (int j = magneticFields.Count - 1; j >= 0; j--)
                {
                    if (accelerations[i].TimeStamp > magneticFields[j].TimeStamp)
                    {
                        nearestSmallerMagneticIndex = j;
                        break;
                    }
                }
                if (nearestSmallerMagneticIndex == -1)
                    continue;



                result.Add(new CombinedDataPoint(accelerations[i].TimeStamp, magneticFields[nearestSmallerMagneticIndex].MagneticSample, GyroscopeDataPoints[nearestSmallerGyroscopeIndex].Rotation, accelerations[i].Acceleration));
            }
            return result;
        }
    }
}