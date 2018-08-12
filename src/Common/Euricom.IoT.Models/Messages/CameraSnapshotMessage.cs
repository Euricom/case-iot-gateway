using System;

namespace Euricom.IoT.Models.Messages
{
    public class CameraSnapshotMessage : CommandMessage
    {
        public Guid CorrelationId { get; set; }
    }
}