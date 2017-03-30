using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common
{
    public class Device
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public HardwareType Type { get; set; } 
    }
}
