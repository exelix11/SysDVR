using SysDVR.Client.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SysDVR.Client.Sources
{
    internal class NetworkScan
    {
        public event Action<DeviceInfo> OnDeviceFound;
        public event Action<string> OnFailure;

        CancellationTokenSource cancel;
        HashSet<IPAddress> devices = new();

        public async void StartScanning() 
        {
            devices.Clear();
            cancel = new CancellationTokenSource();
            using var client = new UdpClient(19999);

            try
            {
                while (!cancel.IsCancellationRequested)
                {
                    var msg = await client.ReceiveAsync(cancel.Token);
                    if (msg.Buffer != null)
                    {
                        var ip = msg.RemoteEndPoint.Address;
                        if (!devices.Contains(ip))
                        {
                            var info = DeviceInfo.TryParse(ConnectionType.Net, msg.Buffer, msg.RemoteEndPoint.Address.ToString());
                            if (info != null)
                            {
                                OnDeviceFound?.Invoke(info);
                                devices.Add(ip);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!cancel.IsCancellationRequested)
                    OnFailure?.Invoke(e.Message);
            }
        }

        public void StopScannning() 
        {
            cancel.Cancel();
        }
    }
}
