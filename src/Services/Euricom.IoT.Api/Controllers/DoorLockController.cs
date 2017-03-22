using Euricom.IoT.Api.Notifications;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.DanaLock;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public sealed partial class DoorLockController
    {
        private readonly DoorLockConfig _config;
        private readonly DanaLock.DanaLock _danaLock;

        public DoorLockController(DoorLockConfig config)
        {
            _config = config;
            _danaLock = DanaLock.DanaLock.Instance;
        }

        [UriFormat("/doorlock/{state}")]
        public IGetResponse Switch(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                return ResponseUtilities.ResponseFail("param state was null or empty");
            }
            else if (state != "open" && state != "close")
            {
                return ResponseUtilities.ResponseFail($"UNKNOWN parameter: { state}. Please use 'on' or 'off'");
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
                        return ResponseUtilities.ResponseFail($"UNKNOWN parameter: { state}. Please use 'open' or 'close'");
                }

                var notification = new DoorLockNotification
                {
                    DeviceKey = _config.DeviceKey,
                    Locked = state == "open" ? true : false,
                    Timestamp = DateTimeHelpers.Timestamp(),
                };

                // Publish to IoT Hub
                PublishDoorLockEvent(notification);

                // Send response back
                return ResponseUtilities.ResponseOk($"OK changed DanaLock device state to : {state}");
            }
            catch (Exception ex)
            {
                return ResponseUtilities.ResponseFail($"LazyBone switch failed, exception: {ex.Message}");
            }
        }

        private void PublishDoorLockEvent(DoorLockNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher(_config.DeviceKey).Publish(json);
        }
    }
}
