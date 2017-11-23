namespace Euricom.IoT.Models
{
    public abstract class ZWaveDevice : Device
    {
        public byte NodeId { get; protected set; }
        
        protected ZWaveDevice(string deviceId, HardwareType type, byte nodeId)
             : base(deviceId, type)
        {
            NodeId = nodeId;
        }
    }
}
