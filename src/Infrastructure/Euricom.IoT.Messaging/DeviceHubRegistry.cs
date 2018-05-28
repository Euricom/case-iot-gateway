using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using Device = Microsoft.Azure.Devices.Device;

namespace Euricom.IoT.Messaging
{
    public class DeviceHubRegistry : IDeviceHubRegistry
    {
        private readonly RegistryManager _registryManager;

        public DeviceHubRegistry(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                Logger.Instance.Warning("Please set a connection string for Azure IoT Hub URI");
                throw new ArgumentException("settings.AzureIotHubUriConnectionString was empty");
            }

            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }

        public async Task<string> AddDeviceAsync(string deviceId, HardwareType type)
        {
            var device = await _registryManager.AddDeviceAsync(new Device(deviceId));
            var twin = await _registryManager.GetTwinAsync(deviceId);

            dynamic properties = new ExpandoObject();
            properties.tags = new Dictionary<string, string>
            {
                { "MessageType", GetType(type) },
                { "Gateway",  "IoTGateway" } //TODO: not hardcoded
            };

            var json = JsonConvert.SerializeObject((IDictionary<string, object>)properties);
            await _registryManager.UpdateTwinAsync(deviceId, json, twin.ETag);

            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        private string GetType(HardwareType type)
        {
            switch (type)
            {
                case HardwareType.WallMountSwitch:
                    return "wallmount_switch";
                case HardwareType.Camera:
                    return "camera";
                case HardwareType.DanaLock:
                    return "danalock";
                case HardwareType.LazyBoneSwitch:
                    return "lazybone_switch";
                case HardwareType.LazyBoneDimmer:
                    return "lazybone_dimmer";
                default:
                    return type.ToString().ToLower();
            }
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