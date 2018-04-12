using System;
using System.Globalization;
using System.Windows.Media;

namespace OpcUa.Client.WPF
{
    public class BooleanToColorConverter : BaseValueConverter<BooleanToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool)value ? Brushes.LimeGreen : Brushes.Red;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool)value;
        }
    }
}