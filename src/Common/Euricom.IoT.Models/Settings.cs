﻿using Euricom.IoT.Models.Logging;

namespace Euricom.IoT.Models
{
    public class Settings
    {
        public int Id { get; set; }

        public Settings()
        {
            GatewayName = "IoTGateway";
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