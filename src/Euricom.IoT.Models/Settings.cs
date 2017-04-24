using Euricom.IoT.Models.Logging;

namespace Euricom.IoT.Models
{
    public class Settings
    {
        public string Password { get; set; }
        public LogLevel LogLevel { get; set; }
        public int HistoryLog { get; set; }
        public string GatewayName
        {
            get
            {
                return "IoTGateway"; //TODO make setting configurable
            }
        }
        public string GatewayDeviceKey { get; set; }
        public string AzureIotHubUri { get; set; }
        public string AzureIotHubUriConnectionString { get; set; }
        public string AzureAccountName { get; set; }
        public string AzureStorageAccessKey { get; set; }
        public string DropboxAccessToken { get; set; }
    }
}
