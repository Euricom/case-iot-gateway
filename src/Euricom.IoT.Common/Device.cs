using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common
{
    public abstract class Device
    {
        public string DeviceKey { get; set; }
        public string Name { get; set; }
    }
}
