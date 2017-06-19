using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models
{
    public class Camera : Device
    {
        public Camera()
        {
            Type = HardwareType.Camera;
        }

        public string Address { get; set; }
        public string DropboxPath { get; set; }
        public int PollingTime { get; set; }
        public int MaximumDaysDropbox { get; set; }
        public double MaximumStorageDropbox { get; set; }
        public int MaximumDaysAzureBlobStorage { get; set; }
    }
}
