namespace Euricom.IoT.Models
{
    public abstract class ZWaveDevice : Device
    {
        public byte NodeId { get; protected set; }
        
        protected ZWaveDevice(HardwareType type, byte nodeId)
             : base(type)
        {
            NodeId = nodeId;
        }
    }
}
