using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Logging;
using Euricom.IoT.Messaging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class WallMountSwitchManager : IWallMountSwitchManager
    {
        private readonly Euricom.IoT.WallMountSwitch.IWallMountSwitchManager _manager;
        private readonly IAzureDeviceManager _azureDeviceManager;

        public WallMountSwitchManager()
        {
            _manager = new Euricom.IoT.WallMountSwitch.WallMountSwitchManager();
            var settings = Database.Instance.GetConfigSettings();
            _azureDeviceManager = new AzureDeviceManager.AzureDeviceManager(settings);
        }

        public Task<IEnumerable<Euricom.IoT.Models.WallMountSwitch>> GetAll()
        {
            var wallmounts = Database.Instance.GetWallMountSwitches();
            return Task.FromResult(wallmounts.AsEnumerable());
        }

        public Task<Euricom.IoT.Models.WallMountSwitch> GetByDeviceId(string deviceId)
        {
            var json = Database.Instance.GetValue(Constants.DBREEZE_TABLE_WALLMOUNTS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Euricom.IoT.Models.WallMountSwitch>(json));
        }

        public Task<Euricom.IoT.Models.WallMountSwitch> GetByDeviceName(string deviceName)
        {
            var deviceId = new HardwareManager().GetDeviceId(deviceName);
            var json = Database.Instance.GetValue(Constants.DBREEZE_TABLE_WALLMOUNTS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Euricom.IoT.Models.WallMountSwitch>(json));
        }

        public async Task<Euricom.IoT.Models.WallMountSwitch> Add(Euricom.IoT.Models.WallMountSwitch wallmount)
        {
            // Add device to Azure Device IoT
            // wallmount.DeviceId = await _azureDeviceManager.AddDeviceAsync(wallmount.Name);
            wallmount.DeviceId = Guid.NewGuid().ToString();

            //Convert to json
            var json = JsonConvert.SerializeObject(wallmount);

            //Save to database
            Database.Instance.SetValue(Constants.DBREEZE_TABLE_WALLMOUNTS, wallmount.DeviceId, json);

            return await GetByDeviceId(wallmount.DeviceId);
        }

        public async Task<Euricom.IoT.Models.WallMountSwitch> Edit(Euricom.IoT.Models.WallMountSwitch wallmount)
        {
            if (wallmount == null)
            {
                throw new ArgumentNullException("wallmount");
            }
            else if (String.IsNullOrEmpty(wallmount.DeviceId))
            {
                throw new ArgumentException("wallmount.DeviceId");
            }

            var json = JsonConvert.SerializeObject(wallmount);

            Database.Instance.SetValue(Constants.DBREEZE_TABLE_WALLMOUNTS, wallmount.DeviceId, json);

            return await GetByDeviceId(wallmount.DeviceId);
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
            catch (Exception)
            {
                throw;
            }
        }

        public bool TestConnection(string deviceId)
        {
            var config = DataLayer.Database.Instance.GetWallMountConfig(deviceId);
            var nodeId = config.NodeId;
            return _manager.TestConnection(nodeId);
        }

        public async Task<bool> IsOn(byte nodeId)
        {
            var wallmount = Database.Instance.GetWallMountSwitches().SingleOrDefault(x => x.NodeId == nodeId);
            if (wallmount == null)
            {
                throw new InvalidOperationException($"Could not find a wallmount by nodeId: {nodeId}");
            }
            return await IsOn(wallmount.DeviceId);
        }

        public async Task<bool> IsOn(string deviceId)
        {
            try
            {
                var config = DataLayer.Database.Instance.GetWallMountConfig(deviceId);
                if (!config.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
                }
                var nodeId = config.NodeId;
                return _manager.IsOn(nodeId);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public async Task Switch(string deviceId, string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new Exception("param state was null or empty");
            }
            else if (state != "on" && state != "off")
            {
                throw new Exception($"UNKNOWN parameter: { state}. Please use 'on' or 'off'");
            }

            try
            {
                var settings = Database.Instance.GetConfigSettings();
                var config = DataLayer.Database.Instance.GetWallMountConfig(deviceId);

                if (!config.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
                }

                var nodeId = config.NodeId;

                switch (state)
                {
                    case "on":
                        _manager.SetOn(nodeId);
                        break;
                    case "off":
                        _manager.SetOff(nodeId);
                        break;
                    default:
                        throw new InvalidOperationException($"unknown operation for Wallmount Switch node: {nodeId}, state: {state}");
                }

                // Log command
                Logger.Instance.LogInformationWithDeviceContext(deviceId, $"Changed state: {state}");

                // Publish to IoT Hub
                var notification = new WallMountSwitchNotification
                {
                    DeviceKey = deviceId,
                    On = state == "on" ? true : false,
                    Timestamp = DateTimeHelpers.Timestamp(),
                };

                PublishWallMountEvent(settings, config.Name, config.DeviceId, notification);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        private void PublishWallMountEvent(Settings settings, string deviceName, string deviceId, WallMountSwitchNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher(settings, deviceName, deviceId).Publish(json);
        }
    }
}
