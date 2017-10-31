namespace Euricom.IoT.Models
{
    public class DanaLock : ZWaveDevice
    {
        public DanaLock(byte nodeId) : base(HardwareType.DanaLock, nodeId)
        { }

        public int PollingTime { get; set; }
    }
}
