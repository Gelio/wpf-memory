using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Memory.Converters
{
    [ValueConversion(typeof(double), typeof(Duration))]
    class DoubleToDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double length = (double) value;

            return new Duration(TimeSpan.FromMilliseconds(length));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Duration duration = (Duration) value;

            return duration.TimeSpan.Milliseconds;
        }
    }
}
