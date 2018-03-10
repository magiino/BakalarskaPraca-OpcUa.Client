using System;
using System.Diagnostics;
using System.Globalization;
using Fasetto.Word;

namespace OpcUA.Client
{
    /// <summary>
    /// Converts the <see cref="Variable"/> value of object to proper type
    /// </summary>
    public class ValueTypeValueConverter : BaseValueConverter<ValueTypeValueConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            // Find the page
            switch (parameter as string)
            {
                case "a":
                    return (int)value;
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
