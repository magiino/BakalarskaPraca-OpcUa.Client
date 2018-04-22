using System;
using System.Diagnostics;
using System.Globalization;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class AtchiveTypeToNumberConverter : BaseValueConverter<AtchiveTypeToNumberConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ArchiveInterval)value)
            {
                case ArchiveInterval.None:
                    return "0";
                case ArchiveInterval.TenSecond:
                    return "10";
                case ArchiveInterval.ThirtySecond:
                    return "30";
                case ArchiveInterval.OneMinute:
                    return "60";
                default:
                    Debugger.Break();
                    return "0";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
