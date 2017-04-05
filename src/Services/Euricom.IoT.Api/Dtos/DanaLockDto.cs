using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Dtos
{
    public class DanaLockDto
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string NodeId { get; set; }
        public int PollingTime { get; set; }
        public bool Enabled { get; set; }
    }
}
