using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Memory.Converters
{
    [ValueConversion(typeof(string), null)]
    class PathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string) value;
            if (string.IsNullOrEmpty(path))
                return null;

            return new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage image = value as BitmapImage;
            if (image == null)
                return null;

            return image.UriSource;
        }
    }
}
