﻿using System;
using System.Globalization;

namespace OpcUa.Client.WPF
{
    public class BooleanConverter : BaseValueConverter<BooleanConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool)value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool)value;
        }
    }
}
