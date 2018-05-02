using System.Threading.Tasks;
using Euricom.IoT.Models.Messages;

namespace Euricom.IoT.Interfaces
{
    public interface IMessageHandler
    {
        Task<bool> HandleDanaLockMessage(string deviceId, DanaLockMessage message);
        Task<bool> HandleWallMountSwitchMessage(string deviceId, WallmountSwitchMessage message);
    }
}