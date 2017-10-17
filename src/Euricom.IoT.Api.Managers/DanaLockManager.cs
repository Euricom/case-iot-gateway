using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Logging;
using Euricom.IoT.Messaging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class DanaLockManager : IDanaLockManager
    {
        private readonly Database _database;
        private readonly DanaLock.IDanaLockManager _manager;

        public DanaLockManager(Database database, DanaLock.IDanaLockManager danaLockManager)
        {
            _database = database;
            _manager = danaLockManager;
        }

        public Task<IEnumerable<Euricom.IoT.Models.DanaLock>> GetAll()
        {
            var danalocks = _database.GetDanaLocks();
            return Task.FromResult(danalocks.AsEnumerable());
        }

        public Task<Euricom.IoT.Models.DanaLock> GetByDeviceId(string deviceId)
        {
            var json = _database.GetValue(Constants.DBREEZE_TABLE_DANALOCKS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Euricom.IoT.Models.DanaLock>(json));
        }

        private string GetDeviceId(string deviceName)
        {
            var device = _database.GetDanaLocks().FirstOrDefault(x => x.Name == deviceName);
            if (device == null)
            {
                throw new Exception($"Could not find deviceName: {deviceName}");
            }
            return device.DeviceId;
        }

        public Task<Models.DanaLock> GetByDeviceName(string deviceName)
        {
            var deviceId = GetDeviceId(deviceName);
            var json = _database.GetValue(Constants.DBREEZE_TABLE_DANALOCKS, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Euricom.IoT.Models.DanaLock>(json));
        }

        public async Task<Euricom.IoT.Models.DanaLock> Add(Euricom.IoT.Models.DanaLock danaLock)
        {
            // Generate Device Id
            danaLock.DeviceId = Guid.NewGuid().ToString();

            //Convert to json
            var json = JsonConvert.SerializeObject(danaLock);

            //Save to database
            _database.SetValue(Constants.DBREEZE_TABLE_DANALOCKS, danaLock.DeviceId, json);

            return await GetByDeviceId(danaLock.DeviceId);
        }

        public async Task<Euricom.IoT.Models.DanaLock> Edit(Euricom.IoT.Models.DanaLock danaLock)
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

            _database.SetValue(Constants.DBREEZE_TABLE_DANALOCKS, danaLock.DeviceId, json);

            return await GetByDeviceId(danaLock.DeviceId);
        }

        public async Task Remove(string deviceName)
        { 
            // Remove device from  database
            var deviceId = GetDeviceId(deviceName);
            _database.RemoveDevice(deviceId);       
        }

        public bool TestConnection(string deviceId)
        {
            var config = _database.GetDanaLockConfig(deviceId);
            var nodeId = config.NodeId;
            return _manager.TestConnection(nodeId);
        }

        public async Task<bool> IsLocked(byte nodeId)
        {
            var danalock = _database.GetDanaLocks().SingleOrDefault(x => x.NodeId == nodeId);
            if (danalock == null)
            {
                throw new InvalidOperationException($"Could not find a danalock by nodeId: {nodeId}");
            }
            return await IsLocked(danalock.DeviceId);
        }

        public async Task<bool> IsLocked(string deviceId)
        {
            try
            {
                var config = _database.GetDanaLockConfig(deviceId);
                if (!config.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
                }
                var nodeId = config.NodeId;
                return _manager.IsLocked(nodeId);
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
            else if (state != "open" && state != "close")
            {
                throw new Exception($"UNKNOWN parameter: { state}. Please use 'open' or 'close'");
            }

            try
            {
                var settings = _database.GetConfigSettings();
                var config = _database.GetDanaLockConfig(deviceId);

                if (!config.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
                }

                var nodeId = config.NodeId;

                switch (state)
                {
                    case "open":
                        _manager.OpenLock(nodeId);
                        break;
                    case "close":
                        _manager.CloseLock(nodeId);
                        break;
                    default:
                        throw new InvalidOperationException($"unknown operation for DanaLock node: {nodeId}, state: {state}");
                }

                // Log command
                Logger.Instance.LogInformationWithDeviceContext(deviceId, $"Changed state: {state}");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        private void PublishDanaLockEvent(Settings settings, string deviceName, string deviceId, DanaLockMessage notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher(settings, deviceName, deviceId).Publish(json);
        }
    }
}
