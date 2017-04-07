using Euricom.IoT.Common;
using Euricom.IoT.Logging;
using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.Text;

namespace Euricom.IoT.Messaging
{
    public class MqttMessagePublisher
    {
        private readonly DeviceClient _azureDeviceClient;

        public MqttMessagePublisher(Settings settings, string deviceName, string deviceKey)
        {
            var iotHubUri = settings.AzureIotHubUri;
            _azureDeviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceName, deviceKey), TransportType.Http1);
        }

        public async void Publish(string json)
        {
            try
            {
                var message = new Message(Encoding.ASCII.GetBytes(json));

                await _azureDeviceClient.SendEventAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
