using Euricom.IoT.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Dtos
{
    public class LogLineDto
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }

        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public string Exception { get; set; }
    }
}
