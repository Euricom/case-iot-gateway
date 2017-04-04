using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Euricom.IoT.LazyBone
{
    public sealed class LazyBone
    {
        //private string _hostName;
        //private short _port;

        private readonly int COMMAND_RELAY_TO_1 = 0x65;
        private readonly int COMMAND_RELAY_TO_0 = 0x6F;
        private readonly int COMMAND_RELAY_STATUS = 0x5B;

        private readonly object _syncRoot = new object();
        private SocketClient _client;

        public LazyBone(SocketClient socketClient)
        {
            _client = socketClient;
        }

        public async Task<bool> TestConnection()
        {
            lock (_syncRoot)
            {
                var response = _client.Connect();
                Debug.WriteLine(response);
                return true;
            }
        }

        public async Task<bool> GetCurrentState()
        {

            lock (_syncRoot)
            {
                var response = _client.Send(HexToString(COMMAND_RELAY_STATUS)).Result;
                Debug.WriteLine(response);
                return true;
            }
        }

        public async Task Switch(bool on)
        {
            lock (_syncRoot)
            {
                string message = HexToString(on == true ? COMMAND_RELAY_TO_1 : COMMAND_RELAY_TO_0);
                var response = _client.Send(message).Result;
                Debug.WriteLine(response);
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
