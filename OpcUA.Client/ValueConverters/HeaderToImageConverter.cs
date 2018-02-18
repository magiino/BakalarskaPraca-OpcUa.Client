using System;
using System.Globalization;
using System.Windows.Media.Imaging;
using Fasetto.Word;

namespace OpcUA.Client
{
    /// <inheritdoc />
    /// <summary>
    /// Converts a full path to a specific image type of a drive, folder or file
    /// </summary>
    public class HeaderToImageConverter : BaseValueConverter<HeaderToImageConverter>
    { 
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Resources/Images/{value}.png"));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
