using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Dtos
{
    public class LazyBoneDto
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public short Port { get; set; }
        public int PollingTime { get; set; }
        public bool Enabled { get; set; }
    }
}
