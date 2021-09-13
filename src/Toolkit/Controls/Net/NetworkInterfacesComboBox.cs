using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

using Avalonia;
using Avalonia.Controls.Primitives;

namespace Aldwych.AvaloniaToolkit.Controls
{
    public class NetworkInterfacesComboBox : TemplatedControl
    {
        public static readonly StyledProperty<IEnumerable<NetworkInterface>> NetworkInterfacesProperty = AvaloniaProperty.Register<NetworkInterfacesComboBox, IEnumerable<NetworkInterface>>(nameof(NetworkInterfaces));

        public static readonly StyledProperty<NetworkInterface> SelectedNetworkInterfaceProperty = AvaloniaProperty.Register<NetworkInterfacesComboBox, NetworkInterface>(nameof(SelectedNetworkInterface));

        public static readonly StyledProperty<string> HeaderProperty = AvaloniaProperty.Register<NetworkInterfacesComboBox, string>(nameof(Header));

        public string Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public IEnumerable<NetworkInterface> NetworkInterfaces
        {
            get => GetValue(NetworkInterfacesProperty);
            private set => SetValue(NetworkInterfacesProperty, value);
        }

        public NetworkInterface SelectedNetworkInterface
        {
            get => GetValue(SelectedNetworkInterfaceProperty);
            set => SetValue(SelectedNetworkInterfaceProperty, value);
        }

        public NetworkInterfacesComboBox()
        {
            RefreshInterfaces();
            NetworkChange.NetworkAddressChanged += (object sender, EventArgs e) =>
            {
                RefreshInterfaces();
            };

            NetworkChange.NetworkAvailabilityChanged += (object sender, NetworkAvailabilityEventArgs e) =>
            {
                RefreshInterfaces();
            };
        }

        private void RefreshInterfaces()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tmpNics = NetworkInterface.GetAllNetworkInterfaces().ToList();

            if (tmpNics == null || !tmpNics.Any())
                return;

            nics.Clear();
            foreach (NetworkInterface adapter in tmpNics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();

                if(adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || 
                    adapter.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet ||
                    adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                nics.Add(adapter);

            }

            NetworkInterfaces = nics;
        }

        private List<NetworkInterface> nics = new List<NetworkInterface>();

    }
}
