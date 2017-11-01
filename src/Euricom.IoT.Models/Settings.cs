using Euricom.IoT.Models.Logging;

namespace Euricom.IoT.Models
{
    public class Settings
    {
        public Settings()
        {
            GatewayName = "IoTGateway";
            ZWaveNetworkKey = "0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10";
        }
        
        public LogLevel LogLevel { get; set; }
        public int HistoryLog { get; set; }
        public string GatewayName { get; set; }
        public string GatewayDeviceKey { get; set; }
        public string AzureIotHubUri { get; set; }
        public string AzureIotHubUriConnectionString { get; set; }
        public string AzureAccountName { get; set; }
        public string AzureStorageAccessKey { get; set; }
        public string DropboxAccessToken { get; set; }
        public string ZWaveNetworkKey { get; set; }
    }
}
