using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Dtos
{
    public class CameraDto
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string DropboxPath { get; set; }
        public int PollingTime { get; set; }
        public int MaximumDaysDropbox { get; set; }
        public double MaximumStorageDropbox { get; set; }
        public int MaximumDaysAzureBlobStorage { get; set; }
        public bool Enabled { get; set; }
    }
}
