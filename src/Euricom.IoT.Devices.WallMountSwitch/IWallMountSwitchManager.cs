using System.Threading.Tasks;

namespace Euricom.IoT.WallMountSwitch
{
    public interface IWallMountSwitchManager
    {
        bool TestConnection(byte nodeId);
        bool IsOn(byte nodeId);
        void SetOff(byte nodeId);
        void SetOn(byte nodeId);
    }
}