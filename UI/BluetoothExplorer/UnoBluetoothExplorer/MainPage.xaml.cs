using InTheHand.Bluetooth;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnoBluetoothExplorer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        async void Button_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<BluetoothService> treeSource = new ObservableCollection<BluetoothService>();
            
            ProgressRing.Visibility = Visibility.Visible;

            try
            {
                var available = await Bluetooth.GetAvailabilityAsync();

                if (available)
                {
                    var device = await Bluetooth.RequestDeviceAsync();
                    if (device != null)
                    {
                        await device.Gatt.ConnectAsync();

                        if (device.Gatt.IsConnected)
                        {
                            var services = await device.Gatt.GetPrimaryServicesAsync();
                            foreach (var service in services)
                            {
                                var serviceName = GattServiceUuids.GetServiceName(service.Uuid);
                                if (string.IsNullOrWhiteSpace(serviceName))
                                    serviceName = service.Uuid.ToString();

                                var serviceNode = new BluetoothService { Name = serviceName };
                                treeSource.Add(serviceNode);
                                var characteristics = await service.GetCharacteristicsAsync();

                                foreach (var characteristic in characteristics)
                                {
                                    var charName = GattCharacteristicUuids.GetCharacteristicName(characteristic.Uuid);
                                    if (string.IsNullOrWhiteSpace(charName))
                                        charName = characteristic.Uuid.ToString();

                                    string value = string.Empty;

                                    if (characteristic.Uuid == GattCharacteristicUuids.BatteryLevel)
                                    {
                                        var rawValue = await characteristic.ReadValueAsync();

                                        value = $"{rawValue[0]}%";
                                    }
                                    else if (characteristic.Uuid == GattCharacteristicUuids.DeviceName |
                                        characteristic.Uuid == GattCharacteristicUuids.FirmwareRevisionString |
                                        characteristic.Uuid == GattCharacteristicUuids.ManufacturerNameString |
                                        characteristic.Uuid == GattCharacteristicUuids.ModelNumberString)
                                    {
                                        var rawValue = await characteristic.ReadValueAsync();

                                        value = System.Text.Encoding.UTF8.GetString(rawValue).Trim();
                                    }
                                    /*else
                                    {
                                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                        for(int i = 0; i < Math.Min(10, rawValue.Length); i++)
                                        {
                                            sb.Append(rawValue[i].ToString("X2"));
                                        }
                                        value = sb.ToString();
                                    }*/

                                    serviceNode.Add(new BluetoothCharacteristic { Name = charName, Properties = characteristic.Properties, Value = value });
                                }
                            }
                        }
                    }

                    BluetoothCVS.Source = treeSource;
                }
            }
            finally
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    ProgressRing.Visibility = Visibility.Collapsed;
                });
            }
        }
    }

    public sealed class BluetoothCharacteristic
    {
        public string Name { get; set; }
        public GattCharacteristicProperties Properties { get; set; }

        public string Value { get; set; }
    }

    public sealed class BluetoothService : ObservableCollection<BluetoothCharacteristic>
    {
        public string Name { get; set; }
    }
}
