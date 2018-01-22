using Euricom.IoT.Devices.ZWave;

namespace Euricom.IoT.DataLayer.Interfaces
{
    public interface IZWaveDeviceRepository
    {
        ZWaveDevice GetZWaveDevice(byte nodeId);

        void UpdateZWaveDevice(ZWaveDevice device);
    }
}
