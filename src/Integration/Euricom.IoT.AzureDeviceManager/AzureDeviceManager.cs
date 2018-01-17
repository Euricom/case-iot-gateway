using Euricom.IoT.Logging;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;
using Newtonsoft.Json;

namespace Euricom.IoT.AzureDeviceManager
{
    public class AzureDeviceManager : IAzureDeviceManager
    {
        private readonly RegistryManager _registryManager;

        public AzureDeviceManager(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                Logger.Instance.LogWarningWithContext(GetType(), "Please set a connection string for Azure IoT Hub URI");
                throw new ArgumentException("settings.AzureIotHubUriConnectionString was empty");
            }

            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        public async Task<IEnumerable<Device>> GetDevicesAsync()
        {
            var devices = await _registryManager.GetDevicesAsync(int.MaxValue);
            return devices;
        }

        public async Task<string> AddDeviceAsync(string deviceId)
        {
            try
            {
                var device = await _registryManager.AddDeviceAsync(new Device(deviceId));
                return device.Authentication.SymmetricKey.PrimaryKey;
            }
            catch (DeviceAlreadyExistsException deviceAlreadyExistsException)
            {
                Logger.Instance.LogErrorWithContext(GetType(), deviceAlreadyExistsException);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }

        public async Task RemoveDeviceAsync(string deviceId)
        {
            try
            {
                var device = await _registryManager.GetDeviceAsync(deviceId);
                if (device != null)
                    await _registryManager.RemoveDeviceAsync(device);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }

        public async Task UpdateStateAsync(string deviceId, Dictionary<string, string> properties)
        {
            try
            {
                var twin = await _registryManager.GetDeviceAsync(deviceId);

                var patch = new
                {
                    properties = new
                    {
                        desired = properties
                    }
                };
                
                await _registryManager.UpdateTwinAsync(deviceId, JsonConvert.SerializeObject(patch), twin.ETag);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }
    }
}
