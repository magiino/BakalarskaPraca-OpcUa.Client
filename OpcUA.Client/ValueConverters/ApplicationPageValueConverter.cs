using System;
using System.Diagnostics;
using System.Globalization;
using OpcUA.Client.Core;

namespace OpcUA.Client
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Find the page
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.Main:
                    return new MainPage();

                case ApplicationPage.Endpoints:
                    return new DiscoverEndpoints();

                case ApplicationPage.ClientSettings:
                    return new UaClientSettings();

                case ApplicationPage.Welcome:
                    return new WelcomePage();
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
