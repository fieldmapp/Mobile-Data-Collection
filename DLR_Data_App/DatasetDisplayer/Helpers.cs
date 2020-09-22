using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DatasetDisplayer
{
    public static class Helpers
    {
        public static Vector3 PointwiseOperation(this Vector3 vec, Func<float, float> op)
        {
            return new Vector3(op(vec.X), op(vec.Y), op(vec.Z));
        }

        public static Vector3 PointwiseDoubleOperation(this Vector3 vec, Func<double, double> op)
        {
            return new Vector3((float)op(vec.X), (float)op(vec.Y), (float)op(vec.Z));
        }

        public static Vector3 Abs(this Vector3 vec) => vec.PointwiseOperation(Math.Abs);
    }
}
