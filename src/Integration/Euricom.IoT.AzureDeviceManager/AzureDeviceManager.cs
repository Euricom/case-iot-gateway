using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Logging;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Euricom.IoT.AzureDeviceManager
{
    public class AzureDeviceManager : IAzureDeviceManager
    {
        private const string ConnectionStringTemplate = "HostName={0};DeviceId={1};SharedAccessKey={2}";
        private readonly string _connectionString;

        public AzureDeviceManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task UpdateStateAsync(string deviceId, string primaryKey, Dictionary<string, object> properties)
        {
            try
            {
                using (var client = DeviceClient.CreateFromConnectionString(string.Format(ConnectionStringTemplate, _connectionString, deviceId, primaryKey), TransportType.Mqtt))
                {
                    var report = JObject.FromObject(properties);
                    var collection = new TwinCollection(JsonConvert.SerializeObject(report, Formatting.None, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));

                    await client.UpdateReportedPropertiesAsync(collection);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }
    }
}