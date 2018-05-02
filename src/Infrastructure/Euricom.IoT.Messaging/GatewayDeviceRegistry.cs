using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Messaging.Interfaces;

namespace Euricom.IoT.Messaging
{
    public class GatewayDeviceRegistry : IGatewayDeviceRegistry, IMonitor
    {
        private readonly IGatewayDeviceFactory _factory;
        private CancellationTokenSource _cancellation;

        private readonly ConcurrentDictionary<string, IGatewayDevice> _devices = new ConcurrentDictionary<string, IGatewayDevice>();

        public GatewayDeviceRegistry(IGatewayDeviceFactory factory)
        {
            _factory = factory;
        }

        public async Task Initialize(Dictionary<string, string> devices)
        {
            foreach (var device in devices)
            {
                await AddDeviceAsync(device.Key, device.Value);
            }
        }

        public Task AddDeviceAsync(string deviceId, string primaryKey)
        {
            var device = _factory.Create(deviceId, primaryKey);

            if (_devices.TryAdd(deviceId, device))
            {
                device.Start();
            }
            
            return Task.CompletedTask;
        }

        public Task RemoveDeviceAsync(string deviceId)
        {
            if (_devices.Remove(deviceId, out IGatewayDevice device))
            {
                device.Stop();
            }

            return Task.CompletedTask;
        }

        public Task SendAsync(string deviceId, Dictionary<string, object> properties)
        {
            if (_devices.TryGetValue(deviceId, out var device))
            {
                device.Send(properties);
            }

            return Task.CompletedTask;
        }

        public void StartMonitoring()
        {
            _cancellation = new CancellationTokenSource();

            Task.Run(async () => await MonitorDevices(), _cancellation.Token);
        }

        private async Task MonitorDevices()
        {
            while (_cancellation.IsCancellationRequested == false)
            {
                foreach (var device in _devices.Values.ToList())
                {
                    if (device.IsRunning() == false)
                    {
                        Logger.Instance.Debug($"GatewayDevice {device.DeviceId} not running. Restarting...");

                        device.Stop();

                        // Device might have been removed in the mean time
                        if (_devices.ContainsKey(device.DeviceId))
                        {
                            device.Start();
                        }
                    }
                }

                await Task.Delay(60000);
            }
        }

        public void StopMonitoring()
        {
            _cancellation.Cancel();
        }
    }
}