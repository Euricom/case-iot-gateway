namespace Euricom.IoT.Models.Notifications
{
    public sealed class WallMountSwitchNotification
    {
        public bool On { get; set; }
        public string Timestamp { get; set; }
        public string DeviceKey { get; set; }
    }
}
