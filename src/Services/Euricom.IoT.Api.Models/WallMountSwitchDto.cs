namespace Euricom.IoT.Api.Models
{
    public class WallMountSwitchDto
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public byte NodeId { get; set; }
        public int PollingTime { get; set; }
        public bool Enabled { get; set; }
    }
}
