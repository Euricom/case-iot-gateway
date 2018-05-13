using System;

namespace Euricom.IoT.Models.Logging
{
    public class LogLine
    {
        public string DeviceId { get; set; }

        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public string Exception { get; set; }
    }
}
