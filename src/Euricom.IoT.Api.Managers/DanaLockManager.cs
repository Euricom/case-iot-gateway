using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.Devices.DanaLock;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using System;

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

        public void Switch(string device, string state)
        {
            var config = DataLayer.Database.Instance.GetLazyBoneConfig(device);

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
                switch (state)
                {
                    case "open":
                        _danaLock.OpenLock();
                        break;
                    case "close":
                        _danaLock.CloseLock();
                        break;
                    default:
                        throw new InvalidOperationException($"unknown operation for DanaLock, state: {state}");
                }

                var notification = new LazyBoneNotification
                {
                    DeviceKey = config.DeviceId,
                    State = state == "open" ? true : false,
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
