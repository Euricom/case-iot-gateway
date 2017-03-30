using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class DevicesController
    {
        private readonly IHardwareManager _hardwareManager;

        public DevicesController()
        {
            _hardwareManager = new HardwareManager();
        }

        [UriFormat("/hardware")]
        public IGetResponse GetHardware()
        {
            var hardware = _hardwareManager.GetHardwareDevices();
            return new GetResponse(GetResponse.ResponseStatus.OK, hardware);
        }

        [UriFormat("/hardware/add?name={name}&type={type}")]
        public IPostResponse AddHardware(string name, string type)
        {
            try
            {
                var newDevice = _hardwareManager.AddHardware(new Device()
                {
                    Name = name,
                    Type = (HardwareType) Enum.Parse(typeof(HardwareType), type),
                });
                return ResponseUtilities.PostResponseOk(newDevice.DeviceId);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.PostResponseFail($"Could not add hardware: exception: {ex.Message}");
            }
        }

        [UriFormat("/hardware/{deviceid}")]
        public IDeleteResponse DeleteHardware(string deviceid)
        {
            try
            {
                var device = _hardwareManager.DeleteHardware(deviceid);
                return ResponseUtilities.DeleteResponseOk(deviceid);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.DeleteResponseFail($"Could not add hardware: exception: {ex.Message}");
            }
        }
    }
}
