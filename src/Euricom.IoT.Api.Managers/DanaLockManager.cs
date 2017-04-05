using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class DanaLockManager : IDanaLockManager
    {
        private readonly ZWave.ZWaveManager _zwaveManager;
        private readonly IAzureDeviceManager _azureDeviceManager;

        public DanaLockManager()
        {
            _zwaveManager = ZWave.ZWaveManager.Instance;
            _azureDeviceManager = new AzureDeviceManager.AzureDeviceManager();
        }

        public Task<IEnumerable<Common.DanaLock>> GetAll()
        {
            var cameras = Database.Instance.GetDanaLocks();
            return Task.FromResult(cameras.AsEnumerable());
        }

        public Task<Common.DanaLock> Get(string deviceId)
        {
            var json = Database.Instance.GetValue(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Common.DanaLock>(json));
        }

        public async Task<Common.DanaLock> Add(Common.DanaLock danaLock)
        {
            //Add device to Azure Device IoT
            //var deviceId = _azureDeviceManager.AddDeviceAsync(camera.Name).Result;
            danaLock.DeviceId = Guid.NewGuid().ToString();

            //Convert to json
            var json = JsonConvert.SerializeObject(danaLock);

            //Save to database
            Database.Instance.SetValue(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS, danaLock.DeviceId, json);

            return await Get(danaLock.DeviceId);
        }

        public async Task<Common.DanaLock> Edit(Common.DanaLock danaLock)
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

            Database.Instance.SetValue(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS, danaLock.DeviceId, json);

            return await Get(danaLock.DeviceId);
        }

        public async Task<bool> Remove(string deviceId)
        {
            try
            {
                // Remove device from Azure
                // await _azureDeviceManager.RemoveDeviceAsync(deviceId);

                // Remove device from  database
                Database.Instance.RemoveDevice(deviceId);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<bool> TestConnection(string deviceId)
        {
            var config = DataLayer.Database.Instance.GetDanaLockConfig(deviceId);
            var nodeId = config.NodeId;
            return _zwaveManager.TestConnection(nodeId);
        }

        public async Task<bool> IsLocked(byte nodeId)
        {
            return _zwaveManager.IsLocked(nodeId);
        }

        public async Task<bool> IsLocked(string deviceId)
        {
            try
            {
                var config = DataLayer.Database.Instance.GetDanaLockConfig(deviceId);
                var nodeId = config.NodeId;
                return _zwaveManager.IsLocked(nodeId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Switch(string deviceId, string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new Exception("param state was null or empty");
            }
            else if (state != "open" && state != "close")
            {
                throw new Exception($"UNKNOWN parameter: { state}. Please use 'open' or 'close'");
            }

            var config = DataLayer.Database.Instance.GetDanaLockConfig(deviceId);

            try
            {
                var nodeId = config.NodeId;
                switch (state)
                {
                    case "open":
                        _zwaveManager.OpenLock(nodeId);
                        break;
                    case "close":
                        _zwaveManager.CloseLock(nodeId);
                        break;
                    default:
                        throw new InvalidOperationException($"unknown operation for DanaLock node: {nodeId}, state: {state}");
                }

                var notification = new DanaLockNotification
                {
                    DeviceKey = deviceId,
                    Locked = state == "closed" ? true : false,
                    Timestamp = DateTimeHelpers.Timestamp(),
                };

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SendDoorLockNotification(string deviceName, string deviceId, DanaLockNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher(deviceName, deviceId).Publish(json);
        }
    }
}
