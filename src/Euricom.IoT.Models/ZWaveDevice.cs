namespace Euricom.IoT.Models
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
