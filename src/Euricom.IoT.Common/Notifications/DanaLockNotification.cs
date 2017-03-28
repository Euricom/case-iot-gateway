namespace Euricom.IoT.Common.Notifications
{
    public sealed class DanaLockNotification
    {
        public bool Locked { get; set; }
        public string Timestamp { get; set; }
        public string DeviceKey { get; set; }
    }
}
