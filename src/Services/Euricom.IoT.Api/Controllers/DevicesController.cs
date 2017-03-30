using Euricom.IoT.Api.Dtos;
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

        [UriFormat("/hardware")]
        public IPostResponse AddHardware([FromContent] DeviceDto deviceDto)
        {
            try
            {
                if (deviceDto == null)
                {
                    throw new ArgumentNullException("deviceDto");
                }

                var newDevice = _hardwareManager.AddHardware(new Device()
                {
                    Name = deviceDto.Name,
                    Type = GetDeviceTypeFromDto(deviceDto),
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

        private HardwareType GetDeviceTypeFromDto(DeviceDto deviceDto)
        {
            switch (deviceDto.Type.ToLower())
            {
                case "camera":
                    return HardwareType.Camera;
                case "danalock":
                    return HardwareType.DanaLock;
                case "lazybone":
                    return HardwareType.LazyBoneSwitch;
            }

            throw new ArgumentException($"Cannot add unknown type: {deviceDto.Type}");
        }
    }
}
