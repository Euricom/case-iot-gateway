using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.AzureDeviceManager
{
    public class AzureDeviceManager : IAzureDeviceManager
    {
        private RegistryManager _registryManager;

        public AzureDeviceManager(Settings settings)
        {
            string connectionString = GetConnectionString(settings);
            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        public async Task<IEnumerable<Microsoft.Azure.Devices.Device>> GetDevicesAsync()
        {
            var devices = await _registryManager.GetDevicesAsync(int.MaxValue);
            return devices;
        }

        public async Task<string> AddDeviceAsync(string deviceName)
        {
            Microsoft.Azure.Devices.Device device;
            try
            {
                device = await _registryManager.AddDeviceAsync(new Microsoft.Azure.Devices.Device(deviceName));
                return device.Authentication.SymmetricKey.PrimaryKey;
            }
            catch (DeviceAlreadyExistsException deviceAlreadyExistsException)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), deviceAlreadyExistsException);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public async Task RemoveDeviceAsync(string deviceName)
        {
            try
            {
                var device = await _registryManager.GetDeviceAsync(deviceName);
                if (device != null)
                    await _registryManager.RemoveDeviceAsync(device);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        private string GetConnectionString(Settings settings)
        {
            if (String.IsNullOrEmpty(settings.AzureIotHubUriConnectionString))
            {
                Logger.Instance.LogWarningWithContext(this.GetType(), "Please set a connection string for Azure IoT Hub URI");
                throw new ArgumentException("settings.AzureIotHubUriConnectionString was empty");
            }
            return settings.AzureIotHubUriConnectionString;
        }
    }
}
