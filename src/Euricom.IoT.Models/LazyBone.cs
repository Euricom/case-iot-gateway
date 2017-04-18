using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models
{
    public class LazyBone : Device
    {
        public string Host { get; set; }
        public short Port { get; set; }
        public int PollingTime { get; set; }
        public bool IsDimmer { get; set; }
    }
}
