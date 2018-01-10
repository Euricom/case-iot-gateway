using Euricom.IoT.Models;

namespace Euricom.IoT.Devices.ZWave
{
    public abstract class ZWaveDevice : Device
    {
        public byte NodeId { get; protected set; }
        
        protected ZWaveDevice(string deviceId, string primaryKey, HardwareType type, byte nodeId)
             : base(deviceId, primaryKey, type)
        {
            NodeId = nodeId;
        }


    }
}
