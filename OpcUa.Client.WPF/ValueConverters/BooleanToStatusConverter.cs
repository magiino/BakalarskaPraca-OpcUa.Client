using System;
using System.Globalization;

namespace OpcUa.Client.WPF
{
    public class BooleanToStatusConverter : BaseValueConverter<BooleanToStatusConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool)value ? "Running" : "Stopped";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool)value;
        }
    }
}