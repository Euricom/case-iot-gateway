using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Models;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.Camera;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class CameraManager : ICameraManager
    {
        private readonly IDeviceRepository<Camera> _repository;
        private readonly IHttpService _httpService;
        private readonly IDeviceHubRegistry _deviceRegistry;
        private readonly IGatewayDeviceRegistry _gatewayDeviceRegistry;

        public CameraManager(IDeviceRepository<Camera> repository, IHttpService httpService, IDeviceHubRegistry deviceRegistry, IGatewayDeviceRegistry gatewayDeviceRegistry)
        {
            _repository = repository;
            _httpService = httpService;
            _deviceRegistry = deviceRegistry;
            _gatewayDeviceRegistry = gatewayDeviceRegistry;
        }

        public IEnumerable<CameraDto> Get()
        {
            var cameras = _repository.Get();
            return Mapper.Map<IEnumerable<CameraDto>>(cameras);
        }

        public CameraDto Get(string deviceId)
        {
            var camera = _repository.Get(deviceId);
            return Mapper.Map<CameraDto>(camera);
        }
        
        public async Task<CameraDto> Add(CameraDto dto)
        {
            var primaryKey = await _deviceRegistry.AddDeviceAsync(dto.DeviceId, HardwareType.Camera);

            var camera = new Camera(dto.DeviceId, primaryKey, dto.Name, dto.Enabled, dto.Address, dto.DropboxPath, dto.PollingTime,
                dto.MaximumDaysDropbox, dto.MaximumStorageDropbox, dto.MaximumDaysAzureBlobStorage);

            _repository.Add(camera);

            await _gatewayDeviceRegistry.AddDeviceAsync(dto.DeviceId, primaryKey);

            return Mapper.Map<CameraDto>(camera);
        }

        public CameraDto Update(CameraDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (String.IsNullOrEmpty(dto.DeviceId))
            {
                throw new ArgumentException("dto.DeviceId");
            }

            var camera = _repository.Get(dto.DeviceId);

            camera.Update(dto.Name, dto.Enabled, dto.Address, dto.DropboxPath, dto.PollingTime, dto.MaximumDaysDropbox,
                dto.MaximumStorageDropbox, dto.MaximumDaysAzureBlobStorage);

            _repository.Update(camera);

            return Mapper.Map<CameraDto>(camera);
        }

        public async Task Remove(string deviceId)
        {
            await _gatewayDeviceRegistry.RemoveDeviceAsync(deviceId);
            await _deviceRegistry.RemoveDeviceAsync(deviceId);

            _repository.Remove(deviceId);
        }

        public Task<bool> TestConnection(string deviceId)
        {
            var device = _repository.Get(deviceId);

            return device.TestConnection(_httpService);
        }

        public void Notify(string deviceId, string url, string timestamp, int frameNumber, int eventNumber)
        {
            var device = _repository.Get(deviceId);


            //if (config.Enabled)
            //{
            //    var notification = new CameraMotionMessage
            //    {
            //        Gateway = "IoTGateway",
            //        Device = config.Name,
            //        CommandToken = null,
            //        MessageType = MessageTypes.Camera,
            //        FilePath = url,
            //        EventNumber = eventNumber,
            //        FrameNumber = frameNumber,
            //    };

            //    // Publish to IoT Hub
            //    PublishMotionEvent(settings, config.Name, config.DeviceId, notification);
            //}
        }
    }
}
