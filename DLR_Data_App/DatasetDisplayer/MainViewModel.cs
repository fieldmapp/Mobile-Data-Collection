using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatasetDisplayer
{
    public class MainViewModel
    {
        public struct AccelerationDataPoint
        {
            public TimeSpan TimeStamp;
            public Vector3 Acceleration;

            public AccelerationDataPoint(TimeSpan timeStamp, Vector3 acceleration)
            {
                TimeStamp = timeStamp;
                Acceleration = acceleration;
            }
        }

        public struct OrientationDataPoint
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

        public MainViewModel()
        {
            this.MyModel = new PlotModel { Title = "Example 1" };
            var accelerationJson = File.ReadAllText("accelerationData.json");
            var orientationJson = File.ReadAllText("orientationData.json");
            var accelerations = JsonTranslator.GetFromJson<List<AccelerationDataPoint>>(accelerationJson);
            var orientations = JsonTranslator.GetFromJson<List<OrientationDataPoint>>(orientationJson);
            var orientedAccelerations = MatchOrientationAndAccelerations(accelerations, orientations);
            var filteredAccelerations = ApplyRollingAverageToAcceleration(orientedAccelerations, 50);
            var velocities = CalculateVelocities(filteredAccelerations);
            //var filteredVelocities = ApplyRollingAverageToVelocities(velocities);
            //AddAccelerationSeries(orientedAccelerations);
            //AddAccelerationSeries(filteredAccelerations);
            //AddVelocitySeries(velocities);
            AddDistanceSeries(velocities);
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

                if (nearestSmallerOrientationIndex == accelerations.Count - 1)
                {
                    result.Add(new CombinedDataPoint(accelerations[i].TimeStamp, Vector3.Transform(accelerations[i].Acceleration, orientations[accelerations.Count - 1].Rotation)));
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

        private List<VelocityDataPoint> CalculateVelocities(List<CombinedDataPoint> transformedDataList)
        {
            var result = new List<VelocityDataPoint>();
            Vector3 Velocity = Vector3.Zero;
            Vector3 MovedDistance = Vector3.Zero;
            const float G = 9.80665f;
            Vector3[] InitSteps = new Vector3[InitEnd];
            Vector3 ConstantAcceleration = new Vector3();
            Vector3 AccelerationStandardVariance = new Vector3();

            for (int i = 0; i < transformedDataList.Count; i++)
            {
                if (i < InitEnd)
                {
                    InitSteps[i] = transformedDataList[i].OrientatedAcceleration;
                }
                if (i == InitEnd)
                {
                    var sampleSum = Vector3.Zero;
                    for (int j = 0; j < InitEnd; j++)
                    {
                        sampleSum += InitSteps[j];
                    }

                    ConstantAcceleration = sampleSum / InitEnd;
                    var sampleDerivationSum = Vector3.Zero;
                    for (int j = 0; j < InitEnd; j++)
                    {
                        sampleDerivationSum += (InitSteps[j] - ConstantAcceleration).Abs();
                    }
                    AccelerationStandardVariance = sampleDerivationSum.PointwiseDoubleOperation(Math.Sqrt) / InitEnd;
                }
                if (i >= InitEnd)
                {
                    var velocityChange = transformedDataList[i].OrientatedAcceleration - ConstantAcceleration;
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

                    var elapsedSeconds = (float)(transformedDataList[i].TimeStamp - transformedDataList[i - 1].TimeStamp).TotalSeconds;

                    Velocity += velocityChange * G * elapsedSeconds;

                    result.Add(new VelocityDataPoint(transformedDataList[i].TimeStamp, Velocity));
                }
            }

            return result;
        }

        private void AddAccelerationSeries(List<CombinedDataPoint> transformedDataList)
        {
            var xSeries = new LineSeries();
            var ySeries = new LineSeries();
            var zSeries = new LineSeries();
            foreach (var transformedDataPoint in transformedDataList)
            {
                xSeries.Points.Add(new OxyPlot.DataPoint(transformedDataPoint.TimeStamp.TotalSeconds, transformedDataPoint.OrientatedAcceleration.X));
                ySeries.Points.Add(new OxyPlot.DataPoint(transformedDataPoint.TimeStamp.TotalSeconds, transformedDataPoint.OrientatedAcceleration.Y));
                zSeries.Points.Add(new OxyPlot.DataPoint(transformedDataPoint.TimeStamp.TotalSeconds, transformedDataPoint.OrientatedAcceleration.Z));
            }

            MyModel.Series.Add(xSeries);
            MyModel.Series.Add(ySeries);
            MyModel.Series.Add(zSeries);
        }
        
        public PlotModel MyModel { get; private set; }

        const int InitEnd = 1400;

        public void AddVelocitySeries(List<VelocityDataPoint> velocityDataPoints)
        {
            var xSeries = new LineSeries();
            var ySeries = new LineSeries();
            var zSeries = new LineSeries();

            for (int i = 0; i < velocityDataPoints.Count; i++)
            {
                xSeries.Points.Add(new DataPoint(velocityDataPoints[i].TimeStamp.TotalSeconds, velocityDataPoints[i].OrientatedVelocity.X));
                ySeries.Points.Add(new DataPoint(velocityDataPoints[i].TimeStamp.TotalSeconds, velocityDataPoints[i].OrientatedVelocity.Y));
                zSeries.Points.Add(new DataPoint(velocityDataPoints[i].TimeStamp.TotalSeconds, velocityDataPoints[i].OrientatedVelocity.Z));
            }
            
            MyModel.Series.Add(xSeries);
            MyModel.Series.Add(ySeries);
            MyModel.Series.Add(zSeries);
        }

        public void AddDistanceSeries(List<VelocityDataPoint> velocityDataPoints)
        {
            var xSeries = new LineSeries();
            var ySeries = new LineSeries();
            var zSeries = new LineSeries();

            Vector3 Movement = Vector3.Zero;

            for (int i = 1; i < velocityDataPoints.Count; i++)
            {
                var elapsedSeconds = (float)(velocityDataPoints[i].TimeStamp - velocityDataPoints[i - 1].TimeStamp).TotalSeconds;
                Movement += velocityDataPoints[i].OrientatedVelocity * elapsedSeconds;

                xSeries.Points.Add(new OxyPlot.DataPoint(velocityDataPoints[i].TimeStamp.TotalSeconds, Movement.X));
                ySeries.Points.Add(new OxyPlot.DataPoint(velocityDataPoints[i].TimeStamp.TotalSeconds, Movement.Y));
                zSeries.Points.Add(new OxyPlot.DataPoint(velocityDataPoints[i].TimeStamp.TotalSeconds, Movement.Z));
            }

            MyModel.Series.Add(xSeries);
            MyModel.Series.Add(ySeries);
            MyModel.Series.Add(zSeries);
        }
    }
}
