using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Models;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.Camera;
using Euricom.IoT.Http.Interfaces;

namespace Euricom.IoT.Api.Managers
{
    public class CameraManager : ICameraManager
    {
        private readonly IDeviceRepository<Camera> _repository;
        private readonly IHttpService _httpService;

        public CameraManager(IDeviceRepository<Camera> repository, IHttpService httpService)
        {
            _repository = repository;
            _httpService = httpService;
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
        
        public CameraDto Add(CameraDto dto)
        {
            var camera = new Camera(dto.Name, dto.Enabled, dto.Address, dto.DropboxPath, dto.PollingTime,
                dto.MaximumDaysDropbox, dto.MaximumStorageDropbox, dto.MaximumDaysAzureBlobStorage);

            _repository.Add(camera);

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

        public void Remove(string deviceId)
        {
            _repository.Remove(deviceId);
        }

        //public void Notify(string deviceId, string url, string timestamp, int frameNumber, int eventNumber)
        //{
        //    var settings = _settingsRepository.Get();
        //    var config = _database.GetCameraConfig(deviceId);
        //    if (config.Enabled)
        //    {
        //        var notification = new CameraMotionMessage
        //        {
        //            Gateway = "IoTGateway",
        //            Device = config.Name,
        //            CommandToken = null,
        //            MessageType = MessageTypes.Camera,
        //            FilePath = url,
        //            EventNumber = eventNumber,
        //            FrameNumber = frameNumber,
        //        };

        //        // Publish to IoT Hub
        //        PublishMotionEvent(settings, config.Name, config.DeviceId, notification);
        //    }
        //}

        //public async void UploadFilesToBlobStorage(string path, Dictionary<string, byte[]> files)
        //{
        //    foreach (var file in files)
        //    {
        //        using (MemoryStream ms = new MemoryStream(file.Value))
        //        {
        //            await _azureBlobStorageManager.PostImage(path, file.Key, ms);
        //        }
        //    }
        //}

        public Task<bool> TestConnection(string deviceId)
        {
            var device = _repository.Get(deviceId);

            return device.TestConnection(_httpService);
        }

        //private void PublishMotionEvent(Settings settings, string deviceName, string deviceKey, CameraMotionMessage notification)
        //{
        //    var json = JsonConvert.SerializeObject(notification);
        //    new MqttMessagePublisher(settings, deviceName, deviceKey).Publish(json);
        //}
    }
}
