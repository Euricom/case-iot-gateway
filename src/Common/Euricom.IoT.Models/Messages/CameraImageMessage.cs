using System;

namespace Euricom.IoT.Models.Messages
{
    public class CameraImageMessage : DeviceMessage
    {
        public string Url { get; set; }
        public string Timestamp { get; set; }
        public Guid? CorrelationId { get; set; }
        public bool Motion { get; set; }

        public CameraImageMessage() : base("camera_image")
        {
        }
    }
}
