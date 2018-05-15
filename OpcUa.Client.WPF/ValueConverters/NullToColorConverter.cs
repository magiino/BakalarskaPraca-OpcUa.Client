using System;
using System.Globalization;
using System.Windows.Media;

namespace OpcUa.Client.WPF
{
    public class NullToColorConverter : BaseValueConverter<NullToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Brushes.LimeGreen : Brushes.Red;
        }
         
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
