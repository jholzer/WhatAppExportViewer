using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace WhatsBack
{
    public class ImageFileToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (string)value;
            if (!File.Exists(path))
                return null;

            var imageSource = ImageSource.FromFile(path);
            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}