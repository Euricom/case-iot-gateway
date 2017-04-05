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
        //public string Username { get; set; }
        //public string Password { get; set; }
        public string DropboxPath { get; set; }
        public int PollingTime { get; set; }
        public bool Enabled { get; set; }
    }
}
