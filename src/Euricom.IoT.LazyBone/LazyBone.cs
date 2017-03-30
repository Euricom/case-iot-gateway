using Euricom.IoT.Common;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Euricom.IoT.LazyBone
{
    public sealed class LazyBone
    {
        private Common.LazyBone _config;

        private readonly int COMMAND_RELAY_TO_1 = 0x65;
        private readonly int COMMAND_RELAY_TO_0 = 0x6F;
        private readonly int COMMAND_RELAY_STATUS = 0x5B;

        public LazyBone()
        {
        }

        public void SetConfig(Common.LazyBone config)
        {
            _config = config;
        }

        public async Task<bool> GetCurrentState(string deviceId)
        {
            //var host = _config.Host;
            //var port = _config.Port;
            var host = "10.0.1.127";
            var port = "2000";
            string response;

            using (SocketClient client = new SocketClient())
            {
                await client.Connect(_config.Host, _config.Port, HexToString(COMMAND_RELAY_STATUS));
                response = await client.Read(); //Response should be a single byte .. If bit is high , relay is in pos 1

                // TODO debug response and check whether response is bit high
                return true;
            }
        }

        public async Task Switch(bool on)
        {
            //var host = _config.Host;
            //var port = _config.Port;
            var host = "10.0.1.127";
            var port = "2000";
            string response;

            using (SocketClient client = new SocketClient())
            {
                var command = on == true ? COMMAND_RELAY_TO_1 : COMMAND_RELAY_TO_0;
                await client.Connect(host, port, HexToString(command));
                response = await client.Read();
            }
        }

        private string HexToString(int hex)
        {
            int value = Convert.ToInt32(hex);
            string stringValue = Char.ConvertFromUtf32(value);
            return stringValue;
        }
    }
}
