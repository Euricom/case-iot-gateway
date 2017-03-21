using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.Text;

namespace Euricom.IoT.Messaging
{
    public class MqttMessagePublisher
    {
        private readonly DeviceClient _azureDeviceClient;

        private const string iotHubUri = "eurismartoffice.azure-devices.net";
        private const string deviceKey = "y8VBjCDsGl7XM6UxRWfZNieYMVqC20gLSZLJN0n9rTE=";

        public MqttMessagePublisher(string deviceKey)
        {
            _azureDeviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice", deviceKey), TransportType.Mqtt);
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
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
