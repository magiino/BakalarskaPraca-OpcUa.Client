using System;
using System.Diagnostics;
using System.Globalization;
using OpcUA.Client.Core;

namespace OpcUA.Client
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public class ProtocolValueConverter : BaseValueConverter<ProtocolValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Protocol)value)
            {
                case Protocol.UaTcp:
                    return "UA Tcp";

                case Protocol.UaHttps:
                    return "UA Https";

                default:
                    Debugger.Break();
                    return null;
            }


            // Find the page
            /*
            switch (value)
            {
                case "UA Tcp":
                    return Protocol.UaTcp;

                case "UA Https":
                    return Protocol.UaHttps;

                default:
                    Debugger.Break();
                    return null;
            }
            */
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Protocol.UaTcp;
            /*
            switch ((value as ComboBoxItem)?.Name)
            {
                case "UA Tcp":
                    return Protocol.UaTcp;

                case "UA Https":
                    return Protocol.UaHttps;

                default:
                    Debugger.Break();
                    return null;
            }
            */
        }
    }
}
