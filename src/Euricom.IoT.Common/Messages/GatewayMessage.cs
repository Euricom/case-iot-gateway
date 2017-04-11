using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common.Messages
{
    public class GatewayMessage
    {
        public string CommandToken { get; set; }
        public string DeviceId { get; set; }
        public string Message { get; set; }
    }
}
