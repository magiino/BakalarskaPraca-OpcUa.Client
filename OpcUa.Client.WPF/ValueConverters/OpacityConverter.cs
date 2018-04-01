using System;
using System.Globalization;

namespace OpcUa.Client.WPF
{
    public class OpacityConverter : BaseValueConverter<OpacityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value != null && (bool)value ? 0.3d : 0d;
            return val;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
