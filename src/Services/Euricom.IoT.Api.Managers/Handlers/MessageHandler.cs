using System.Threading.Tasks;
using Euricom.IoT.DataLayer.Interfaces;
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

        public MessageHandler(IZWaveController zWaveController, IDeviceRepository<DanaLock> danalockRepository, IDeviceRepository<WallMountSwitch> wallmountSwitchRepository)
        {
            _zWaveController = zWaveController;
            _danalockRepository = danalockRepository;
            _wallmountSwitchRepository = wallmountSwitchRepository;
        }

        public Task<bool> HandleDanaLockMessage(string deviceId, DanaLockMessage message)
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

            return Task.FromResult(true);
        }

        public Task<bool> HandleWallMountSwitchMessage(string deviceId, WallmountSwitchMessage message)
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

            return Task.FromResult(true);
        }
    }
}
