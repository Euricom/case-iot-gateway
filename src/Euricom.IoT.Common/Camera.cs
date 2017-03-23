using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common
{
    public class Camera : Device
    {
        public string Address { get; set; }
        public string UserName { get; set; }
        public bool Enabled { get; set; }
    }
}
