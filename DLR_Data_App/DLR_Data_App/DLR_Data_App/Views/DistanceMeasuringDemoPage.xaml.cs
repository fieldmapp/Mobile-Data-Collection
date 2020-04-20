using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DistanceMeasuringDemoPage : ContentPage
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

        struct OrientationDataPoint
        {
            public TimeSpan TimeStamp;
            public Quaternion Rotation;

            public OrientationDataPoint(TimeSpan timeStamp, Quaternion rotation)
            {
                TimeStamp = timeStamp;
                Rotation = rotation;
            }
        }

        List<AccelerationDataPoint> AccelerometerDataPoints = new List<AccelerationDataPoint>();
        List<OrientationDataPoint> OrientationDataPoints = new List<OrientationDataPoint>();

        Stopwatch Timekeeper = new Stopwatch();

        bool Started = false;

        public DistanceMeasuringDemoPage()
        {
            InitializeComponent();
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            if (Started)
            {
                Timekeeper.Stop();
                Sensor.Instance.OrientationSensor.ReadingChanged -= OrientationSensor_ReadingChanged;
                Sensor.Instance.Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
                var accelerations = JsonTranslator.GetJson(AccelerometerDataPoints);
                var orientations = JsonTranslator.GetJson(OrientationDataPoints);
            }
            else
            {
                Started = true;
                Timekeeper.Start();
                Sensor.Instance.OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
                Sensor.Instance.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            }
        }

        private void OrientationSensor_ReadingChanged(object sender, Xamarin.Essentials.OrientationSensorChangedEventArgs e)
        {
            OrientationDataPoints.Add(new OrientationDataPoint(Timekeeper.Elapsed, e.Reading.Orientation));
        }

        private void Accelerometer_ReadingChanged(object sender, Xamarin.Essentials.AccelerometerChangedEventArgs e)
        {
            AccelerometerDataPoints.Add(new AccelerationDataPoint(Timekeeper.Elapsed, e.Reading.Acceleration));
        }
    }
}