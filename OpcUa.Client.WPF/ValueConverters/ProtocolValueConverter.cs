using System;
using System.Diagnostics;
using System.Globalization;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public class ProtocolValueConverter : BaseValueConverter<ProtocolValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((TransportProtocol)value)
            {
                case TransportProtocol.UaTcp:
                    return "UA Tcp";

                case TransportProtocol.UaHttps:
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
                    return TransportProtocol.UaTcp;

                case "UA Https":
                    return TransportProtocol.UaHttps;

                default:
                    Debugger.Break();
                    return null;
            }
            */
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TransportProtocol.UaTcp;
            /*
            switch ((value as ComboBoxItem)?.Name)
            {
                case "UA Tcp":
                    return TransportProtocol.UaTcp;

                case "UA Https":
                    return TransportProtocol.UaHttps;

                default:
                    Debugger.Break();
                    return null;
            }
            */
        }
    }
}
