using System;
using System.Diagnostics;
using System.Globalization;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
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
                    return new MainPage(new MainViewModel(IoC.UnitOfWork, IoC.UaClientApi, IoC.Messenger));

                case ApplicationPage.Endpoints:
                    return new DiscoverEndpoints(new DiscoverEndpointsViewModel(IoC.UnitOfWork, IoC.UaClientApi));

                case ApplicationPage.Welcome:
                    return new WelcomePage(new WelcomeViewModel(IoC.UnitOfWork, IoC.UaClientApi, IoC.Messenger));
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
