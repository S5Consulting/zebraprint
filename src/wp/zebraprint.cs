using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace WPCordovaClassLib.Cordova.Commands
{
    public class zebraprint : BaseCommand
    {        
        private StreamSocket socket = null;
        private DataWriter printWriter = null;
        private RfcommDeviceService printService = null;
        private BluetoothDevice bluetoothDevice;

        public void print(string options)
        {
            string mac = JSON.JsonHelper.Deserialize<string[]>(options)[0];
            string data = JSON.JsonHelper.Deserialize<string[]>(options)[1];

            DoPrint(mac, data);
        }

        private async void DoPrint(string mac, string data)
        {
            PluginResult result;

            try
            {
                ulong macAsNumber = Convert.ToUInt64(mac, 16);
                bluetoothDevice = await BluetoothDevice.FromIdAsync(mac);
            }
            catch (Exception ex)
            {
                result = new PluginResult(PluginResult.Status.ERROR, "ERROR");
                DispatchCommandResult(result);
                return;
            }
            
            var rfcommServices = await bluetoothDevice.GetRfcommServicesForIdAsync(
                RfcommServiceId.FromUuid(Guid.Parse("00001101-0000-1000-8000-00805F9B34FB")), BluetoothCacheMode.Uncached);

            if (rfcommServices.Services.Count > 0)
            {
                printService = rfcommServices.Services[0];
            }
            else
            {
                result = new PluginResult(PluginResult.Status.ERROR, upperCase);
                DispatchCommandResult(result);
                return;
            }

            lock (this)
            {
                socket = new StreamSocket();
            }

            try
            {
                await socket.ConnectAsync(printService.ConnectionHostName, printService.ConnectionServiceName);

                printWriter = new DataWriter(socket.OutputStream);
                printWriter.WriteBytes(System.Text.Encoding.ASCII.GetBytes(data));

                await printWriter.StoreAsync();

                result = new PluginResult(PluginResult.Status.OK, "OK");
            }
            catch (Exception ex)
            {
                result = new PluginResult(PluginResult.Status.ERROR, "ERROR");
            }
            finally
            {
                if (printWriter != null)
                {
                    printWriter.DetachStream();
                    printWriter = null;
                }

                if (printService != null)
                {
                    printervice.Dispose();
                    printService = null;
                }
                lock (this)
                {
                    if (socket != null)
                    {
                        socket.Dispose();
                        socket = null;
                    }
                }

                DispatchCommandResult(result);
            }
        }
    }
}
