using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Converters
{
    class BoolToColorConverter : IValueConverter
    {
        public object Convert(object valueObject, Type targetType, object parameterObject, System.Globalization.CultureInfo culture)
        {
            if (!(valueObject is bool value))
                throw new ArgumentException("Parameter needs to be a boolean.", nameof(valueObject));
            var parameterSplit = parameterObject.ToString().Split(',');
            if (parameterSplit.Length != 6)
                throw new ArgumentException("Parameter must to be in format eg \".2,.5,.1,.4,.5,.6\"", 
                    nameof(parameterObject));

            var colorValues = parameterSplit.Select(p => System.Convert.ToSingle(p)).ToArray();

            return value ? new Color(colorValues[0], colorValues[1], colorValues[2]) 
                : new Color(colorValues[3], colorValues[4], colorValues[5]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
