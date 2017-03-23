using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common
{
    public class Settings
    {
        public short PreserveHistoryLog { get; set; } //in weeks
        public string AzureIotHubName { get; set; }
        public string AzureUsername { get; set; }

    }
}
