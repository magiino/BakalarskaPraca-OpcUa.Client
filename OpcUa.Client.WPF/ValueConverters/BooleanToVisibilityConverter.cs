using System;
using System.Globalization;
using System.Windows;

namespace OpcUa.Client.WPF
{
    public class BooleanToVisibilityConverter : BaseValueConverter<BooleanToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter == null)
                return value != null && (bool) value ? Visibility.Visible : Visibility.Collapsed;
            else
                return value != null && (bool) value ? Visibility.Collapsed : Visibility.Visible;
        }
         
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
