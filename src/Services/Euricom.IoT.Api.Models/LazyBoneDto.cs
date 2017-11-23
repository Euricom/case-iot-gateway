namespace Euricom.IoT.Api.Models
{
    public class LazyBoneDto
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public short Port { get; set; }
        public int PollingTime { get; set; }
        public bool IsDimmer { get; set; }
        public bool Enabled { get; set; }
    }
}
