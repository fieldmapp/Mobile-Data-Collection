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
        public struct DataPoint
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

        public struct TransformedDataPoint
        {
            public TimeSpan TimeStamp;
            public Vector3 OrientatedAcceleration;

            public TransformedDataPoint(TimeSpan timeStamp, Vector3 orientatedAcceleration)
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
            var dataJson = File.ReadAllText("data.json");
            var dataList = JsonTranslator.GetFromJson<List<DataPoint>>(dataJson);
            var transformedDataList = dataList.Select(d => new TransformedDataPoint(d.TimeStamp, Vector3.Transform(d.Acceleration, d.Orientation))).ToList();
            var velocities = CalculateVelocities(transformedDataList);
            //AddAccelerationSeries(transformedDataList);
            //AddVelocitySeries(transformedDataList);
            AddDistanceSeries(velocities);
        }

        private List<VelocityDataPoint> CalculateVelocities(List<TransformedDataPoint> transformedDataList)
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

        private void AddAccelerationSeries(List<TransformedDataPoint> transformedDataList)
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

        const int InitEnd = 555;

        public void AddVelocitySeries(List<VelocityDataPoint> velocityDataPoints)
        {
            var xSeries = new LineSeries();
            var ySeries = new LineSeries();
            var zSeries = new LineSeries();

            for (int i = 0; i < velocityDataPoints.Count; i++)
            {
                xSeries.Points.Add(new OxyPlot.DataPoint(velocityDataPoints[i].TimeStamp.TotalSeconds, velocityDataPoints[i].OrientatedVelocity.X));
                ySeries.Points.Add(new OxyPlot.DataPoint(velocityDataPoints[i].TimeStamp.TotalSeconds, velocityDataPoints[i].OrientatedVelocity.Y));
                zSeries.Points.Add(new OxyPlot.DataPoint(velocityDataPoints[i].TimeStamp.TotalSeconds, velocityDataPoints[i].OrientatedVelocity.Z));
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
