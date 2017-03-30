using Euricom.IoT.Common.Secrets;
using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.Text;

namespace Euricom.IoT.Messaging
{
    public class MqttMessagePublisher
    {
        private readonly DeviceClient _azureDeviceClient;

        private const string iotHubUri = Secrets.AZURE_IOT_HUB_URI;

        public MqttMessagePublisher(string deviceName, string deviceKey)
        {
            _azureDeviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice", Secrets.MY_FIRST_DEVICE_KEY), TransportType.Mqtt);
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
