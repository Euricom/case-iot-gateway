using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Models;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;
using System;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
    [RestController(InstanceCreationType.Singleton)]
    public class DevicesController
    {
        private readonly IHardwareManager _hardwareManager;

        public DevicesController(IHardwareManager hardwareManager)
        {
            _hardwareManager = hardwareManager;
        }

        [UriFormat("/hardware")]
        public async Task<IGetResponse> GetHardware()
        {
            try
            {
                var hardware = _hardwareManager.GetHardwareDevices();
                return ResponseUtilities.GetResponseOk(hardware);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not add hardware: exception: {ex.Message}");
            }
        }

        [UriFormat("/hardware")]
        public async Task<IPostResponse> AddHardware([FromContent] DeviceDto deviceDto)
        {
            try
            {
                if (deviceDto == null)
                {
                    throw new ArgumentNullException("deviceDto");
                }

                var newDevice = await _hardwareManager.AddHardware(new Device()
                {
                    Name = deviceDto.Name,
                    Type = GetDeviceTypeFromDto(deviceDto),
                });
                return ResponseUtilities.PostResponseOk(newDevice);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not add hardware: exception: {ex.Message}");
            }
        }

        [UriFormat("/hardware/{devicename}")]
        public async Task<IDeleteResponse> DeleteHardware(string devicename)
        {
            try
            {
                await _hardwareManager.DeleteHardware(devicename);
                return ResponseUtilities.DeleteResponseOk();
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not add hardware: exception: {ex.Message}");
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
