using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models
{
    public class Device
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public HardwareType Type { get; set; } 
        public bool Enabled { get; set; }
    }
}
