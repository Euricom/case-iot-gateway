namespace Euricom.IoT.Api.Models
{
    public class CameraDto
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string DropboxPath { get; set; }
        public int PollingTime { get; set; }
        public bool Enabled { get; set; }
        public string MotionEyeIdentifier { get; set; }
    }
}
