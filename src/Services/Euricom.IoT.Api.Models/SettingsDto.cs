namespace Euricom.IoT.Api.Models
{
    public class SettingsDto
    {
        public string Password { get; set; }
        public string LogLevel { get; set; }
        public int HistoryLog { get; set; }
        public string GatewayDeviceKey { get; set; }
        public string AzureIotHubUri { get; set; }
        public string AzureIotHubUriConnectionString { get; set; }
        public string AzureAccountName { get; set; }
        public string AzureStorageAccessKey { get; set; }
        public string DropboxAccessToken { get; set; }
        public string ZWaveNetworkKey { get; set; }
    }
}
