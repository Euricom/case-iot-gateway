using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models
{
    public class WallMountSwitch : ZWaveDevice
    {
        public WallMountSwitch()
        {
            Type = HardwareType.WallmountSwitch;
        }

        public int PollingTime { get; set; }
    }
}
