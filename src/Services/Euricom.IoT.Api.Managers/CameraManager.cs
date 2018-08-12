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
using Euricom.IoT.Models.Messages;

namespace Euricom.IoT.Api.Managers
{
    public class CameraManager : ICameraManager
    {
        private readonly IDeviceRepository<Camera> _repository;
        private readonly IHttpService _httpService;
        private readonly IDeviceHubRegistry _deviceRegistry;
        private readonly IGatewayDeviceRegistry _gatewayDeviceRegistry;
        private readonly IStorageManager _storageManager;

        public CameraManager(IDeviceRepository<Camera> repository, 
            IHttpService httpService, 
            IDeviceHubRegistry deviceRegistry, 
            IGatewayDeviceRegistry gatewayDeviceRegistry,
            IStorageManager storageManager)
        {
            _repository = repository;
            _httpService = httpService;
            _deviceRegistry = deviceRegistry;
            _gatewayDeviceRegistry = gatewayDeviceRegistry;
            _storageManager = storageManager;
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

            var camera = new Camera(dto.DeviceId, primaryKey, dto.Name, dto.MotionEyeIdentifier, dto.Enabled, dto.Address, dto.DropboxPath, dto.PollingTime);

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

            camera.Update(dto.Name, dto.MotionEyeIdentifier, dto.Enabled, dto.Address, dto.DropboxPath, dto.PollingTime);

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

        public async Task Notify(string deviceId, string fileName, DateTime timestamp)
        {
            var device = _repository.Get(deviceId);

            var picture = await device.GetPicture(_httpService, fileName);

            var location = await _storageManager.PostImage(deviceId, $"{timestamp:yyyy-MM-dd}/{timestamp:O}_{Guid.NewGuid():D}.jpg", picture);

            picture.Dispose();

            await _gatewayDeviceRegistry.SendMessageAsync(device.DeviceId, new CameraImageMessage
            {
                Url = location,
                Timestamp = timestamp.ToString("O"),
                Motion = true
            });
        }

        public async Task<string> GetPicture(string deviceId, Guid? correlationId)
        {
            var device = _repository.Get(deviceId);
            var picture = await device.GetPicture(_httpService);

            var now = DateTime.UtcNow;
            var location = await _storageManager.PostImage(deviceId, $"{now:yyyy-MM-dd}/{now:O}_{correlationId ?? Guid.NewGuid():D}.jpg", picture);

            picture.Dispose();
            
            await _gatewayDeviceRegistry.SendMessageAsync(device.DeviceId, new CameraImageMessage
            {
                Url = location,
                Timestamp = now.ToString("O"),
                CorrelationId = correlationId,
                Motion = false
            });

            return location;
        }
    }
}
