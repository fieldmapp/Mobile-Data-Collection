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
        struct DataPoint
        {
            public TimeSpan TimeStamp;
            public Vector3 Acceleration;
            public Quaternion Orientation;

            public DataPoint(TimeSpan timeStamp, Vector3 acceleration, Quaternion orientation)
            {
                TimeStamp = timeStamp;
                Acceleration = acceleration;
                Orientation = orientation;
            }
        }

        List<DataPoint> DataPoints = new List<DataPoint>();

        Stopwatch Timekeeper = new Stopwatch();

        bool Started = false;

        public DistanceMeasuringDemoPage()
        {
            InitializeComponent();
        }

        private async void StartButton_Clicked(object sender, EventArgs e)
        {
            if (Started)
                return;

            Timekeeper.Start();
            Sensor.Instance.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            await Task.Delay(MeasuringDuration);
            Sensor.Instance.Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
            var output = JsonTranslator.GetJson(DataPoints);
        }

        private void Accelerometer_ReadingChanged(object sender, Xamarin.Essentials.AccelerometerChangedEventArgs e)
        {
            DataPoints.Add(new DataPoint(Timekeeper.Elapsed, e.Reading.Acceleration, Sensor.Instance.OrientationSensor.Orientation));
        }
    }
}