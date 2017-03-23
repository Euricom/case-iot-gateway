using Euricom.IoT.Api.Notifications;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class LazyBoneController
    {
        private readonly LazyBone.LazyBone _lazyBone;

        public LazyBoneController()
        {
            _lazyBone = new LazyBone.LazyBone();
        }

        /// <summary>
        /// Sends a switch command to a specific LazyBone device 
        /// </summary>
        /// <param name="device">Guid of device</param>
        /// <param name="state">on or off</param>
        /// <returns></returns>
        //[UriFormat("/lazybone/{state}")]
        [UriFormat("/lazybone\\?device={device}&state={state}")]
        //public IGetResponse Switch(string state)
        public IGetResponse Switch(string device, string state)
        {
            var config = DataLayer.Database.Instance.GetSwitchConfig(device);

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
                    DeviceKey = config.DeviceKey,
                    State = state == "open" ? true : false,
                    Timestamp = DateTimeHelpers.Timestamp(),
                };

                return ResponseUtilities.ResponseOk($"OK changed LazyBone device state to : {state}");
            }
            catch (Exception ex)
            {
                return ResponseUtilities.ResponseFail($"LazyBone switch failed, exception: {ex.Message}");
            }
        }
    }
}
