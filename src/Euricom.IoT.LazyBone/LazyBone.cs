using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Euricom.IoT.LazyBone
{
    public sealed class LazyBone
    {
        private readonly string COMMAND_RELAY_TO_1 = ((char)(Int16.Parse("0x65", NumberStyles.AllowHexSpecifier))).ToString();
        private readonly string COMMAND_RELAY_TO_0 = ((char)(Int16.Parse("0x6F", NumberStyles.AllowHexSpecifier))).ToString();
        private readonly string COMMAND_RELAY_STATUS = ((char)(Int16.Parse("0x5B", NumberStyles.AllowHexSpecifier))).ToString();

        private LazyBoneConfig _config;

        public LazyBone(LazyBoneConfig config)
        {
            _config = config;
        }


        public async Task<bool> GetCurrentState()
        {
            var host = _config.Host;
            var port = _config.Port;
            string response;

            using (SocketClient client = new SocketClient())
            {
                await client.connect(_config.Host, _config.Port, COMMAND_RELAY_STATUS);
                response = await client.read(); //Response should be a single byte .. If bit is high , relay is in pos 1

                // TODO debug response and check whether response is bit high
                return true;
            }
        }

        public async void Switch(bool on)
        {
            var host = _config.Host;
            var port = _config.Port;
            string response;

            using (SocketClient client = new SocketClient())
            {
                var command = on == true ? COMMAND_RELAY_TO_1 : COMMAND_RELAY_TO_0;
                await client.connect(_config.Host, _config.Port, command);
                response = await client.read();
            }
        }
    }
}
