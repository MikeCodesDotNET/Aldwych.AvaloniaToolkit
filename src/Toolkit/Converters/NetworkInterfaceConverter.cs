using System;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;

using Avalonia.Data.Converters;

namespace Aldwych.AvaloniaToolkit.Converters
{
    public class NetworkInterfaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            else
            {
                if (value is NetworkInterface nic)
                {

                    var ipAddress = (from ip in nic.GetIPProperties().UnicastAddresses
                     where ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                     select ip.Address).SingleOrDefault();

                   
                    return $"{nic.Name} ({ipAddress})";
                }
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
