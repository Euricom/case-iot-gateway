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
            var json = Database.Instance.GetValue(Constants.DBREEZE_TABLE_DANALOCKS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Euricom.IoT.Models.WallMountSwitch>(json));
        }

        public Task<Euricom.IoT.Models.WallMountSwitch> GetByDeviceName(string deviceName)
        {
            var deviceId = new HardwareManager().GetDeviceId(deviceName);
            var json = Database.Instance.GetValue(Constants.DBREEZE_TABLE_DANALOCKS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Euricom.IoT.Models.WallMountSwitch>(json));
        }

        public async Task<Euricom.IoT.Models.WallMountSwitch> Add(Euricom.IoT.Models.WallMountSwitch danaLock)
        {
            //Add device to Azure Device IoT
            danaLock.DeviceId = await _azureDeviceManager.AddDeviceAsync(danaLock.Name);

            //Convert to json
            var json = JsonConvert.SerializeObject(danaLock);

            //Save to database
            Database.Instance.SetValue(Constants.DBREEZE_TABLE_DANALOCKS, danaLock.DeviceId, json);

            return await GetByDeviceId(danaLock.DeviceId);
        }

        public async Task<Euricom.IoT.Models.WallMountSwitch> Edit(Euricom.IoT.Models.WallMountSwitch danaLock)
        {
            if (danaLock == null)
            {
                throw new ArgumentNullException("danaLock");
            }
            else if (String.IsNullOrEmpty(danaLock.DeviceId))
            {
                throw new ArgumentException("danaLock.DeviceId");
            }

            var json = JsonConvert.SerializeObject(danaLock);

            Database.Instance.SetValue(Constants.DBREEZE_TABLE_DANALOCKS, danaLock.DeviceId, json);

            return await GetByDeviceId(danaLock.DeviceId);
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
            //var danalock = Database.Instance.GetDanaLocks().SingleOrDefault(x => x.NodeId == nodeId);
            //if (danalock == null)
            //{
            //    throw new InvalidOperationException($"Could not find a danalock by nodeId: {nodeId}");
            //}
            //return await IsLocked(danalock.DeviceId);

            return false;
        }

        public async Task Switch(string deviceId, string state)
        {
            //if (string.IsNullOrEmpty(state))
            //{
            //    throw new Exception("param state was null or empty");
            //}
            //else if (state != "on" && state != "off")
            //{
            //    throw new Exception($"UNKNOWN parameter: { state}. Please use 'open' or 'close'");
            //}

            //try
            //{
            //    var settings = Database.Instance.GetConfigSettings();
            //    var config = DataLayer.Database.Instance.get(deviceId);

            //    if (!config.Enabled)
            //    {
            //        Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
            //        throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
            //    }

            //    var nodeId = config.NodeId;

            //    switch (state)
            //    {
            //        case "open":
            //            _zwaveManager.OpenLock(nodeId);
            //            break;
            //        case "close":
            //            _zwaveManager.CloseLock(nodeId);
            //            break;
            //        default:
            //            throw new InvalidOperationException($"unknown operation for DanaLock node: {nodeId}, state: {state}");
            //    }

            //    // Log command
            //    Logger.Instance.LogInformationWithDeviceContext(deviceId, $"Changed state: {state}");

            //    // Publish to IoT Hub
            //    var notification = new DanaLockNotification
            //    {
            //        DeviceKey = deviceId,
            //        Locked = state == "closed" ? true : false,
            //        Timestamp = DateTimeHelpers.Timestamp(),
            //    };

            //    PublishDanaLockEvent(settings, config.Name, config.DeviceId, notification);
            //}
            //catch (Exception ex)
            //{
            //    Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
            //    throw;
            //}
        }

        // private void PublishWallMountEvent(Settings settings, string deviceName, string deviceId, ff notification)
        //{
        //    var json = JsonConvert.SerializeObject(notification);
        //    new MqttMessagePublisher(settings, deviceName, deviceId).Publish(json);
        //}
    }
}
