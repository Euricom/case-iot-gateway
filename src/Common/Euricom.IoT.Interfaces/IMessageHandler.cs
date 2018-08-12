using System.Threading.Tasks;
using Euricom.IoT.Models.Messages;

namespace Euricom.IoT.Interfaces
{
    public interface IMessageHandler
    {
        Task HandleDanaLockMessage(string deviceId, DanaLockMessage message);
        Task HandleWallMountSwitchMessage(string deviceId, WallmountSwitchMessage message);
        Task<CameraImageMessage> HandleCameraMessage(string deviceId, CameraSnapshotMessage message);
    }
}