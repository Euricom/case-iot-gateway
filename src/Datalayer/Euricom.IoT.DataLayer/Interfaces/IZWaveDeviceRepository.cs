using System.Collections.Generic;
using Euricom.IoT.Devices.ZWave;

namespace Euricom.IoT.DataLayer.Interfaces
{
    public interface IZWaveDeviceRepository
    {
        List<ZWaveDevice> GetDevices();

        ZWaveDevice GetZWaveDevice(byte nodeId);

        void UpdateZWaveDevice(ZWaveDevice device);
    }
}
