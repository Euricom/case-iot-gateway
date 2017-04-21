using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models
{
    public class DanaLock : ZWaveDevice
    {
        public DanaLock()
        {
            Type = HardwareType.DanaLock;
        }

        public int PollingTime { get; set; }
    }
}
