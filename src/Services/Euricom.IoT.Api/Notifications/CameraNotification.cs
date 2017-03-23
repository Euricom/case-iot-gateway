namespace Euricom.IoT.Api.Notifications
{
    public sealed class CameraNotification
    {
        public string FilePath { get; set; }

        public int FrameNumber { get; set; }

        public int EventNumber { get; set; }

        public string Timestamp { get; set; }

        public string DeviceKey { get; set; }
    }
}
