using Euricom.IoT.Interfaces;
using Euricom.IoT.Messaging.Interfaces;

namespace Euricom.IoT.Messaging
{
    public class GatewayDeviceFactory : IGatewayDeviceFactory
    {
        private readonly string _host;
        private readonly IMessageHandler _handler;

        public GatewayDeviceFactory(IMessageHandler handler, string host)
        {
            _host = host;
            _handler = handler;
        }

        public IGatewayDevice Create(string deviceId, string primaryKey)
        {
            return new GatewayDevice(_handler, _host, deviceId, primaryKey);
        }
    }
}