using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.DataLayer;
using Euricom.IoT.LazyBone;
using Euricom.IoT.Logging;
using Euricom.IoT.Messaging;
using Euricom.IoT.Models.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Euricom.IoT.Models;
using Euricom.IoT.Common;

namespace Euricom.IoT.Api.Managers
{
    public class LazyBoneManager : ILazyBoneManager
    {
        private IAzureDeviceManager _azureDeviceManager;


        public LazyBoneManager()
        {
            var settings = Database.Instance.GetConfigSettings();
            _azureDeviceManager = new AzureDeviceManager.AzureDeviceManager(settings);
        }

        public Task<IEnumerable<Euricom.IoT.Models.LazyBone>> GetAll()
        {
            var lazyBones = Database.Instance.GetLazyBones();
            return Task.FromResult(lazyBones.AsEnumerable());
        }

        public Task<Euricom.IoT.Models.LazyBone> GetByDeviceId(string deviceId)
        {
            var json = Database.Instance.GetValue(Constants.DBREEZE_TABLE_LAZYBONES, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Euricom.IoT.Models.LazyBone>(json));
        }

        public Task<Euricom.IoT.Models.LazyBone> GetByDeviceName(string deviceName)
        {
            var deviceId = new HardwareManager().GetDeviceId(deviceName);
            var json = Database.Instance.GetValue(Constants.DBREEZE_TABLE_LAZYBONES, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Euricom.IoT.Models.LazyBone>(json));
        }

        public async Task<Euricom.IoT.Models.LazyBone> Add(Euricom.IoT.Models.LazyBone lazyBone)
        {
            //Add device to Azure Device IoT
            lazyBone.DeviceId = await _azureDeviceManager.AddDeviceAsync(lazyBone.Name);

            //Convert to json
            var json = JsonConvert.SerializeObject(lazyBone);

            //Save to database
            Database.Instance.SetValue("LazyBones", lazyBone.DeviceId, json);

            return lazyBone;
        }

        public async Task<Euricom.IoT.Models.LazyBone> Edit(Euricom.IoT.Models.LazyBone lazyBone)
        {
            if (lazyBone == null)
            {
                throw new ArgumentNullException("lazyBone");
            }
            else if (String.IsNullOrEmpty(lazyBone.DeviceId))
            {
                throw new ArgumentException("lazyBone.DeviceId");
            }

            var json = JsonConvert.SerializeObject(lazyBone);

            Database.Instance.SetValue(Constants.DBREEZE_TABLE_LAZYBONES, lazyBone.DeviceId, json);

            return await GetByDeviceId(lazyBone.DeviceId);
        }

        public async Task<bool> Remove(string deviceName)
        {
            try
            {
                // Remove device from Azure
                await _azureDeviceManager.RemoveDeviceAsync(deviceName);

                // Remove device from  database
                var deviceId = new HardwareManager().GetDeviceId(deviceName);
                Database.Instance.RemoveDevice(deviceId);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public async Task<string> TestConnection(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            var config = Database.Instance.GetLazyBoneConfig(deviceId);
            return await LazyBoneConnectionManager.Instance.TestConnection(deviceId, config);
        }

        public async Task<bool> GetCurrentState(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            try
            {
                var config = Database.Instance.GetLazyBoneConfig(deviceId);
                if (!config.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
                }
                return await LazyBoneConnectionManager.Instance.GetCurrentState(deviceId, config);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }

        }

        public async Task Switch(string deviceId, string state)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }
            else if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException("state");
            }
            else if (state != "on" && state != "off")
            {
                throw new ArgumentException($"UNKNOWN parameter: { state}. Please use 'on' or 'off'");
            }

            try
            {
                var settings = Database.Instance.GetConfigSettings();
                var config = Database.Instance.GetLazyBoneConfig(deviceId);

                if (!config.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
                }

                switch (state)
                {
                    case "on":
                        await LazyBoneConnectionManager.Instance.Switch(deviceId, config, true);
                        break;
                    case "off":
                        await LazyBoneConnectionManager.Instance.Switch(deviceId, config, false);
                        break;
                    default:
                        throw new InvalidOperationException($"unknown operation for LazyBone, state: {state}");
                }

                // Log command
                Logger.Instance.LogInformationWithDeviceContext(deviceId, $"Changed state: {state}");

                // Publish to IoT Hub
                var notification = new LazyBoneNotification
                {
                    DeviceKey = deviceId,
                    State = state == "on" ? true : false,
                    Timestamp = Common.Utilities.DateTimeHelpers.Timestamp(),
                };
                PublishLazyBoneEvent(settings, config.Name, config.DeviceId, notification);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        private void PublishLazyBoneEvent(Settings settings, string deviceName, string deviceKey, LazyBoneNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher(settings, deviceName, deviceKey).Publish(json);
        }
    }
}
