using System;

namespace Euricom.IoT.Api.Models
{
    public class LogLineDto
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public string Exception { get; set; }
    }
}
