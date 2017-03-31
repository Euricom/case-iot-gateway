using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common
{
    public class Settings
    {
        public int HistoryLog { get; set; }
        public string AzureIotHubUri { get; set; }
        public string AzureIotHubUriConnectionString { get; set; }
        public string AzureAccountName { get; set; }
        public string AzureStorageAccessKey { get; set; }
        public string DropboxAccessToken { get; set; }
    }
}
