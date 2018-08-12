using System;
using System.Threading.Tasks;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.Camera;
using Euricom.IoT.Devices.DanaLock;
using Euricom.IoT.Devices.WallMountSwitch;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Models.Messages;

namespace Euricom.IoT.Api.Managers.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IZWaveController _zWaveController;
        private readonly IDeviceRepository<DanaLock> _danalockRepository;
        private readonly IDeviceRepository<WallMountSwitch> _wallmountSwitchRepository;
        private readonly IDeviceRepository<Camera> _cameraRepository;
        private readonly IHttpService _httpService;
        private readonly IStorageManager _storageManager;

        public MessageHandler(IZWaveController zWaveController, 
            IDeviceRepository<DanaLock> danalockRepository, 
            IDeviceRepository<WallMountSwitch> wallmountSwitchRepository, 
            IDeviceRepository<Camera> cameraRepository, 
            IHttpService httpService, 
            IStorageManager storageManager)
        {
            _zWaveController = zWaveController;
            _danalockRepository = danalockRepository;
            _wallmountSwitchRepository = wallmountSwitchRepository;
            _cameraRepository = cameraRepository;
            _httpService = httpService;
            _storageManager = storageManager;
        }

        public Task HandleDanaLockMessage(string deviceId, DanaLockMessage message)
        {
            var danalock = _danalockRepository.Get(deviceId);

            switch (message.Locked)
            {
                case false:
                    danalock.OpenLock(_zWaveController);
                    break;
                case true:
                    danalock.CloseLock(_zWaveController);
                    break;
            }

            return Task.CompletedTask;
        }

        public Task HandleWallMountSwitchMessage(string deviceId, WallmountSwitchMessage message)
        {
            var device = _wallmountSwitchRepository.Get(deviceId);

            switch (message.State)
            {
                case true:
                    device.TurnOn(_zWaveController);
                    break;
                case false:
                    device.TurnOff(_zWaveController);
                    break;
            }

            return Task.CompletedTask;
        }

        public async Task<CameraImageMessage> HandleCameraMessage(string deviceId, CameraSnapshotMessage message)
        {
            var device = _cameraRepository.Get(deviceId);

            var picture = await device.GetPicture(_httpService);

            var now = DateTime.UtcNow;
            var location = await _storageManager.PostImage(deviceId, $"{now:yyyy-MM-dd}/{now:O}_{message.CorrelationId:D}.jpg", picture);

            picture.Dispose();

            return new CameraImageMessage
            {
                Url = location,
                Timestamp = now.ToString("O"),
                CorrelationId = message.CorrelationId,
                Motion = false
            };
        }
    }
}
