using DLR_Data_App.Localizations;
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

        public struct CombinedDataPoint
        {
            public TimeSpan TimeStamp;
            public Vector3 OrientatedAcceleration;

            public CombinedDataPoint(TimeSpan timeStamp, Vector3 orientatedAcceleration)
            {
                TimeStamp = timeStamp;
                OrientatedAcceleration = orientatedAcceleration;
            }
        }

        public struct VelocityDataPoint
        {
            public TimeSpan TimeStamp;
            public Vector3 OrientatedVelocity;

            public VelocityDataPoint(TimeSpan timeStamp, Vector3 orientatedVelocity)
            {
                TimeStamp = timeStamp;
                OrientatedVelocity = orientatedVelocity;
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

        private async void StartButton_Clicked(object sender, EventArgs e)
        {
            if (Started)
            {
                Timekeeper.Stop();
                Sensor.Instance.OrientationSensor.ReadingChanged -= OrientationSensor_ReadingChanged;
                Sensor.Instance.Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;

                var orientedAccelerations = MatchOrientationAndAccelerations(AccelerometerDataPoints, OrientationDataPoints);
                var calibratedAccelerations = CalibrateAccelerations(orientedAccelerations, 650);
                var filteredAccelerations = ApplyRollingAverageToAcceleration(calibratedAccelerations, 10);
                var velocities = CalculateVelocities(filteredAccelerations);
                var traveledDistance = GetFinalDistance(velocities);
                CenterText.Text = traveledDistance.ToString();
                CenterText.IsVisible = true;
                AccelerometerDataPoints = new List<AccelerationDataPoint>();
                OrientationDataPoints = new List<OrientationDataPoint>();
                StartButton.Text = AppResources.start;
                Started = false;
                Layout.BackgroundColor = Color.White;
                Timekeeper.Reset();
            }
            else
            {
                Started = true;
                Timekeeper.Start();
                Sensor.Instance.OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
                Sensor.Instance.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                StartButton.IsVisible = false;
                Layout.BackgroundColor = Color.Red;
                CenterText.Text = AppResources.calibrationpleasehold;
                CenterText.IsVisible = true;
                await Task.Delay(TimeSpan.FromSeconds(3));
                var calibrationSteps = AccelerometerDataPoints.Count;
                Layout.BackgroundColor = Color.Green;
                CenterText.IsVisible = false;
                StartButton.Text = AppResources.finish;
                StartButton.IsVisible = true;
            }
        }

        private Vector3 GetFinalDistance(List<VelocityDataPoint> velocityDataPoints)
        {
            Vector3 Movement = Vector3.Zero;

            for (int i = 1; i < velocityDataPoints.Count; i++)
            {
                var elapsedSeconds = (float)(velocityDataPoints[i].TimeStamp - velocityDataPoints[i - 1].TimeStamp).TotalSeconds;
                Movement += velocityDataPoints[i].OrientatedVelocity * elapsedSeconds;
            }
            return Movement;
        }

        private void OrientationSensor_ReadingChanged(object sender, Xamarin.Essentials.OrientationSensorChangedEventArgs e)
        {
            OrientationDataPoints.Add(new OrientationDataPoint(Timekeeper.Elapsed, e.Reading.Orientation));
        }

        private void Accelerometer_ReadingChanged(object sender, Xamarin.Essentials.AccelerometerChangedEventArgs e)
        {
            AccelerometerDataPoints.Add(new AccelerationDataPoint(Timekeeper.Elapsed, e.Reading.Acceleration));
        }

        private List<CombinedDataPoint> MatchOrientationAndAccelerations(List<AccelerationDataPoint> accelerations, List<OrientationDataPoint> orientations)
        {
            var result = new List<CombinedDataPoint>();
            int skipped = 0;
            for (int i = 0; i < accelerations.Count; i++)
            {
                var nearestSmallerOrientationIndex = -1;
                for (int j = orientations.Count - 1; j >= 0; j--)
                {
                    if (accelerations[i].TimeStamp > orientations[j].TimeStamp)
                    {
                        nearestSmallerOrientationIndex = j;
                        break;
                    }
                }
                if (nearestSmallerOrientationIndex == -1)
                {
                    //what now?
                    skipped++;
                    continue;
                }

                if (nearestSmallerOrientationIndex == orientations.Count - 1)
                {
                    result.Add(new CombinedDataPoint(accelerations[i].TimeStamp, Vector3.Transform(accelerations[i].Acceleration, orientations[orientations.Count - 1].Rotation)));
                    continue;
                }

                //lerp between orientations[nearestSmallerOrientationIndex] and orientations[nearestSmallerOrientationIndex + 1]
                var durationBetweenOrientationFrames = (orientations[nearestSmallerOrientationIndex + 1].TimeStamp - orientations[nearestSmallerOrientationIndex].TimeStamp).TotalSeconds;
                var durationToNextOrientationFrame = (orientations[nearestSmallerOrientationIndex + 1].TimeStamp - accelerations[i].TimeStamp).TotalSeconds;
                var fractionToNextFrame = durationToNextOrientationFrame / durationBetweenOrientationFrames;
                var lerpedRotation = Quaternion.Lerp(orientations[nearestSmallerOrientationIndex].Rotation, orientations[nearestSmallerOrientationIndex + 1].Rotation, (float)fractionToNextFrame);

                result.Add(new CombinedDataPoint(accelerations[i].TimeStamp, Vector3.Transform(accelerations[i].Acceleration, lerpedRotation)));
            }
            return result;
        }

        private List<CombinedDataPoint> ApplyRollingAverageToAcceleration(List<CombinedDataPoint> accelerations, int halfWidth)
        {
            var result = new List<CombinedDataPoint>();
            for (int i = 0; i < halfWidth; i++)
            {
                result.Add(accelerations[i]);
            }
            for (int i = halfWidth; i < accelerations.Count - halfWidth; i++)
            {
                var localSum = Vector3.Zero;
                for (int j = -halfWidth; j < halfWidth; j++)
                {
                    localSum += accelerations[i + j].OrientatedAcceleration;
                }
                result.Add(new CombinedDataPoint(accelerations[i].TimeStamp, localSum / (halfWidth * 2f)));
            }
            for (int i = accelerations.Count - halfWidth; i < accelerations.Count; i++)
            {
                result.Add(accelerations[i]);
            }
            return result;
        }

        private List<VelocityDataPoint> ApplyRollingAverageToVelocities(List<VelocityDataPoint> velocities, int halfWidth)
        {
            var result = new List<VelocityDataPoint>();
            for (int i = 0; i < halfWidth; i++)
            {
                result.Add(velocities[i]);
            }
            for (int i = halfWidth; i < velocities.Count - halfWidth; i++)
            {
                var localSum = Vector3.Zero;
                for (int j = -halfWidth; j < halfWidth; j++)
                {
                    localSum += velocities[i + j].OrientatedVelocity;
                }
                result.Add(new VelocityDataPoint(velocities[i].TimeStamp, localSum / (halfWidth * 2f)));
            }
            for (int i = velocities.Count - halfWidth; i < velocities.Count; i++)
            {
                result.Add(velocities[i]);
            }
            return result;
        }

        private List<CombinedDataPoint> CalibrateAccelerations(List<CombinedDataPoint> transformedDataList, int initEnd)
        {
            var result = new List<CombinedDataPoint>();
            //var result = new List<VelocityDataPoint>();
            Vector3 Velocity = Vector3.Zero;
            Vector3 MovedDistance = Vector3.Zero;
            Vector3[] InitSteps = new Vector3[initEnd];
            Vector3 ConstantAcceleration = new Vector3();
            Vector3 AccelerationStandardVariance = new Vector3();

            for (int i = 0; i < transformedDataList.Count; i++)
            {
                if (i < initEnd)
                {
                    InitSteps[i] = transformedDataList[i].OrientatedAcceleration;
                }
                if (i == initEnd)
                {
                    var sampleSum = Vector3.Zero;
                    for (int j = 0; j < initEnd; j++)
                    {
                        sampleSum += InitSteps[j];
                    }

                    ConstantAcceleration = sampleSum / initEnd;
                    var sampleDerivationSum = Vector3.Zero;
                    for (int j = 0; j < initEnd; j++)
                    {
                        sampleDerivationSum += (InitSteps[j] - ConstantAcceleration).Abs();
                    }
                    AccelerationStandardVariance = sampleDerivationSum.PointwiseDoubleOperation(Math.Sqrt) / initEnd;
                }
                if (i >= initEnd)
                {
                    var acceleration = transformedDataList[i].OrientatedAcceleration - ConstantAcceleration;
                    //float xAcceleration = acceleration.X,
                    //    yAcceleration = acceleration.Y,
                    //    zAcceleration = acceleration.Z;
                    //if (Math.Abs(xAcceleration) < AccelerationStandardVariance.X)
                    //    xAcceleration = 0;
                    //if (Math.Abs(yAcceleration) < AccelerationStandardVariance.Y)
                    //    yAcceleration = 0;
                    //if (Math.Abs(zAcceleration) < AccelerationStandardVariance.Z)
                    //    zAcceleration = 0;
                    //acceleration = new Vector3(xAcceleration, yAcceleration, zAcceleration);

                    result.Add(new CombinedDataPoint(transformedDataList[i].TimeStamp, acceleration));
                }
            }

            return result;
        }

        public List<VelocityDataPoint> CalculateVelocities(List<CombinedDataPoint> calibratedAccelerations)
        {
            var velocity = Vector3.Zero;
            const float G = 9.80665f;
            var result = new List<VelocityDataPoint>();

            for (int i = 1; i < calibratedAccelerations.Count; i++)
            {
                var elapsedSeconds = (float)(calibratedAccelerations[i].TimeStamp - calibratedAccelerations[i - 1].TimeStamp).TotalSeconds;

                velocity += (calibratedAccelerations[i].OrientatedAcceleration + (calibratedAccelerations[i].OrientatedAcceleration - calibratedAccelerations[i - 1].OrientatedAcceleration).Abs() / 2f) * G * elapsedSeconds;

                result.Add(new VelocityDataPoint(calibratedAccelerations[i].TimeStamp, velocity));
            }

            return result;
        }
    }
}