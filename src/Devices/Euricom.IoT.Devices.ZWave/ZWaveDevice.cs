using System.Collections.Generic;
using Euricom.IoT.Models;

namespace Euricom.IoT.Devices.ZWave
{
    public abstract class ZWaveDevice : Device
    {
        // EF
        protected ZWaveDevice() { }

        public byte NodeId { get; protected set; }
        
        protected ZWaveDevice(string deviceId, string primaryKey, HardwareType type, byte nodeId)
             : base(deviceId, primaryKey, type)
        {
            NodeId = nodeId;
        }

        public abstract bool UpdateState(byte key, byte value);
        public abstract Dictionary<string, object> GetState();
    }
}
