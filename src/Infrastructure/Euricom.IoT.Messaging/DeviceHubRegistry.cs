using System;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Logging;
using Microsoft.Azure.Devices;

namespace Euricom.IoT.Messaging
{
    public class DeviceHubRegistry : IDeviceHubRegistry
    {
        private readonly RegistryManager _registryManager;

        public DeviceHubRegistry(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                Logger.Instance.LogWarningWithContext(GetType(), "Please set a connection string for Azure IoT Hub URI");
                throw new ArgumentException("settings.AzureIotHubUriConnectionString was empty");
            }

            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }
        
        public async Task<string> AddDeviceAsync(string deviceId)
        {
            var device = await _registryManager.AddDeviceAsync(new Device(deviceId));
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public async Task RemoveDeviceAsync(string deviceId)
        {
            var device = await _registryManager.GetDeviceAsync(deviceId);
            if (device != null)
            {
                await _registryManager.RemoveDeviceAsync(device);
            }
        }
    }
}