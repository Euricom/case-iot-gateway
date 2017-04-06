using Euricom.IoT.Common.Logging;
using System;

namespace Euricom.IoT.Common
{
    public class LogLine
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }

        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public string Exception { get; set; }

        public LogProperties Properties { get; set; }
    }
}
