using System;
using System.Globalization;

namespace OpcUa.Client.WPF
{
    public class NullToOpacityConverter : BaseValueConverter<NullToOpacityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value == null ? 0.3d : 1d;
            return val;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
