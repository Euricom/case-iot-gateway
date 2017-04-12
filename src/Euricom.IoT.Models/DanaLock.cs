using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models
{
    public class DanaLock : Device
    {
        public byte NodeId { get; set; } //Example 0x4 (node 4)
        public int PollingTime { get; set; }
    }
}
