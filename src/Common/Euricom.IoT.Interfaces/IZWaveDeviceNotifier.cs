using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IZWaveDeviceNotifier
    {
        void Notify(byte nodeId, byte commandId, byte value);
    }
}
