using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models.Messages
{
    public class DanaLockMessage
    {
        public string DeviceId { get; set; }
        public bool Lock { get; set; }
    }
}
