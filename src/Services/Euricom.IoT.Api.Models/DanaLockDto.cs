namespace Euricom.IoT.Api.Models
{
    public class DanaLockDto
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public byte NodeId { get; set; }
        public int PollingTime { get; set; }
        public bool Enabled { get; set; }
    }
}
