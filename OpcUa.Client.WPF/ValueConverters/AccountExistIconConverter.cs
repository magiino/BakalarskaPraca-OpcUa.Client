using System;
using System.Diagnostics;
using System.Globalization;
using Opc.Ua;

namespace OpcUa.Client.WPF
{
    /// <inheritdoc />
    /// <summary>
    /// Converts a full path to a specific image type of a drive, folder or file
    /// </summary>
    public class AccountExistIconConverter : BaseValueConverter<AccountExistIconConverter>
    { 
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? "AccountRemove" : "Account";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}