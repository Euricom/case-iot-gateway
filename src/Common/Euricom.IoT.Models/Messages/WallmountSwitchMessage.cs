using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models.Messages
{
    public class WallmountSwitchMessage : CommandMessage
    {
        public bool State { get; set; }
    }
}
