namespace Euricom.IoT.Models
{
    public class Camera : Device
    {
        public Camera(): base(HardwareType.Camera)
        { }

        public string Address { get; set; }
        public string DropboxPath { get; set; }
        public int PollingTime { get; set; }
        public int MaximumDaysDropbox { get; set; }
        public double MaximumStorageDropbox { get; set; }
        public int MaximumDaysAzureBlobStorage { get; set; }
    }
}
