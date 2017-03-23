using Euricom.IoT.Api.Notifications;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Euricom.IoT.DanaLock;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class DoorLockController
    {
        private readonly DanaLock.DanaLock _danaLock;

        public DoorLockController()
        {
            _danaLock = DanaLock.DanaLock.Instance;
        }

        [UriFormat("/doorlock/{state}")]
        public IGetResponse Switch(string state) //public IGetResponse Switch(string device, string state)
        {
            //var config = Database.Instance.GetDoorLockConfig(device);

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
                    DeviceKey = "test", //config,
                    Locked = state == "open" ? true : false,
                    Timestamp = DateTimeHelpers.Timestamp(),
                };

                // Send response back
                return ResponseUtilities.ResponseOk($"OK changed DanaLock device state to : {state}");
            }
            catch (Exception ex)
            {
                return ResponseUtilities.ResponseFail($"LazyBone switch failed, exception: {ex.Message}");
            }
        }
    }
}
