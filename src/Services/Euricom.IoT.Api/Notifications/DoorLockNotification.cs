namespace Euricom.IoT.Api.Notifications
{
    public sealed class DoorLockNotification
    {
        public bool Locked { get; set; }
        public string Timestamp { get; set; }
        public string DeviceKey { get; set; }
    }
}
