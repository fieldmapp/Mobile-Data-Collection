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

        struct TransformedDataPoint
        {
            public TimeSpan TimeStamp;
            public Vector3 OrientatedAcceleration;

            public TransformedDataPoint(TimeSpan timeStamp, Vector3 orientatedAcceleration)
            {
                TimeStamp = timeStamp;
                OrientatedAcceleration = orientatedAcceleration;
            }
        }

        public MainViewModel()
        {
            this.MyModel = new PlotModel { Title = "Example 1" };
            var dataJson = File.ReadAllText("data.json");
            var dataList = JsonTranslator.GetFromJson<List<DataPoint>>(dataJson);
            var xSeries = new LineSeries();
            var ySeries = new LineSeries();
            var zSeries = new LineSeries();
            foreach (var transformedDataPoint in dataList.Select(d => new TransformedDataPoint(d.TimeStamp, Vector3.Transform(d.Acceleration, d.Orientation))))
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
    }
}
