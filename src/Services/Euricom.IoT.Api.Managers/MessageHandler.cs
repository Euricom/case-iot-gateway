using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Models.Messages;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;

namespace Euricom.IoT.Api.Managers
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IWallMountSwitchManager _wallMountSwitchManager;
        private readonly IDanaLockManager _danaLockManager;

        public MessageHandler(IDanaLockManager danaLockManager, IWallMountSwitchManager wallMountSwitchManager)
        {
            _danaLockManager = danaLockManager;
            _wallMountSwitchManager = wallMountSwitchManager;
        }

        public Task<bool> HandleDanaLockMessage(string deviceId, DanaLockMessage message)
        {
            _danaLockManager.Switch(deviceId, message.Locked ? "close" : "open");

            return Task.FromResult(true);
        }

        public Task<bool> HandleWallMountSwitchMessage(string deviceId, WallmountSwitchMessage message)
        {
            _wallMountSwitchManager.Switch(deviceId, message.State ? "on" : "off");

            return Task.FromResult(true);
        }
    }
}
