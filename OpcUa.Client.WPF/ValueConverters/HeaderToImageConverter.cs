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
    public class HeaderToImageConverter : BaseValueConverter<HeaderToImageConverter>
    { 
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((NodeClass)value)
            {
                case NodeClass.Object:
                    return "Archive";
                case NodeClass.Unspecified:
                    Debugger.Break();
                    return "HelpCircle";
                case NodeClass.Variable:
                    return "Tag";
                case NodeClass.Method:
                    return "Cube";
                case NodeClass.ObjectType:
                    return "Arch";
                case NodeClass.VariableType:
                    return "Database";
                case NodeClass.ReferenceType:
                    return "RayStartArrow";
                case NodeClass.DataType:
                    return "CodeTags";
                case NodeClass.View:
                    return "Eye";
                default:
                    Debugger.Break();
                    return "Bug";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}