using Euricom.IoT.Api.Configuration;
using Euricom.IoT.Api.Notifications;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public sealed partial class LazyBoneController
    {
        private readonly CameraConfig _config;
        private readonly LazyBone _lazyBone;

        public LazyBoneController(LazyBoneConfig config)
        {
            _lazyBone = new LazyBone(config);
        }

        [UriFormat("/lazybone/{state}")]
        public IGetResponse Switch(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                return ResponseUtilities.ResponseFail("param state was null or empty");
            }
            else if (state != "on" && state != "off")
            {
                return ResponseUtilities.ResponseFail($"UNKNOWN parameter: { state}. Please use 'on' or 'off'");
            }

            try
            {
                switch (state)
                {
                    case "on":
                        _lazyBone.Switch(true);
                        break;
                    case "off":
                        _lazyBone.Switch(false);
                        break;
                    default:
                        throw new InvalidOperationException($"unknown operation for LazyBone, state: {state}");
                }

                var notification = new LazyBoneNotification
                {
                    DeviceKey = _config.DeviceKey,
                    State = state == "open" ? true : false,
                    Timestamp = DateTimeHelpers.Timestamp(),
                };

                // Publish to IoT Hub
                PublishLazyBoneEvent(notification);

                return ResponseUtilities.ResponseOk($"OK changed LazyBone device state to : {state}");
            }
            catch (Exception ex)
            {
                return ResponseUtilities.ResponseFail($"LazyBone switch failed, exception: {ex.Message}");
            }
        }

        private void PublishLazyBoneEvent(LazyBoneNotification notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            new MqttMessagePublisher(_config.DeviceKey).Publish(json);

        }
    }
}
