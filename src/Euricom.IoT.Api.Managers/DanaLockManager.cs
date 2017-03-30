using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.Devices.DanaLock;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class DanaLockManager : IDanaLockManager
    {
        private readonly DanaLock _danaLock;

        public DanaLockManager()
        {
            _danaLock = DanaLock.Instance;
        }

        public Common.DanaLock Add(Common.DanaLock danaLock)
        {
            throw new NotImplementedException();
        }

        public bool IsLocked(byte nodeId)
        {
            return _danaLock.IsLocked(nodeId);
        }

        public bool IsLocked(string deviceId)
        {
            try
            {
                //var config = DataLayer.Database.Instance.GetDanaLockConfig(deviceId);
                //var nodeId = config.NodeId;
                byte nodeId = 0x4;
                return _danaLock.IsLocked(nodeId);
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

            //var config = DataLayer.Database.Instance.GetDanaLockConfig(deviceId);

            try
            {
                //var nodeId = config.NodeId;
                byte nodeId = 0x4;
                switch (state)
                {
                    case "open":
                        _danaLock.OpenLock(nodeId);
                        break;
                    case "close":
                        _danaLock.CloseLock(nodeId);
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
