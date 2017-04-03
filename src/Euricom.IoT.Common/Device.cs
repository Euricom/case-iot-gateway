using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common
{
    public class Device
    {
        //[JsonIgnore]
        public string DeviceId { get; set; }
        public string Name { get; set; }
        //[JsonIgnore]
        public HardwareType Type { get; set; } 
    }
}
