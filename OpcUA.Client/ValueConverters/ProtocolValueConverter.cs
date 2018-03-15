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
            switch ((EProtocol)value)
            {
                case EProtocol.UaTcp:
                    return "UA Tcp";

                case EProtocol.UaHttps:
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
                    return EProtocol.UaTcp;

                case "UA Https":
                    return EProtocol.UaHttps;

                default:
                    Debugger.Break();
                    return null;
            }
            */
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EProtocol.UaTcp;
            /*
            switch ((value as ComboBoxItem)?.Name)
            {
                case "UA Tcp":
                    return EProtocol.UaTcp;

                case "UA Https":
                    return EProtocol.UaHttps;

                default:
                    Debugger.Break();
                    return null;
            }
            */
        }
    }
}
