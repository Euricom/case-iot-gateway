using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.LazyBone
{
    public sealed class LazyBone
    {
        private readonly int COMMAND_GET_SOFTWARE_VERSION = 0x5A;
        private readonly int COMMAND_RELAY_TO_1 = 0x65;
        private readonly int COMMAND_RELAY_TO_0 = 0x6F;
        private readonly int COMMAND_RELAY_STATUS = 0x5B;

        private SocketClient _client;

        public LazyBone(SocketClient socketClient)
        {
            _client = socketClient;
        }

        public async Task<string> TestConnection()
        {
            lock (_client)
            {
                var response = _client.Send(HexToString(COMMAND_GET_SOFTWARE_VERSION), true).Result;
                var lazyBoneSoftwareVersion = GetLazyBoneSoftwareVersion(response);
                return lazyBoneSoftwareVersion;
            }
        }

        public async Task<bool> GetCurrentState()
        {
            lock (_client)
            {
                var response = _client.Send(HexToString(COMMAND_RELAY_STATUS), true).Result;
                var isRelayOn = IsRelayOn(response);
                return true;
            }
        }

        public async Task Switch(bool on)
        {
            lock (_client)
            {
                string message = HexToString(on == true ? COMMAND_RELAY_TO_1 : COMMAND_RELAY_TO_0);
                var response = _client.Send(message, false).Result;
                Debug.WriteLine(response);
            }
        }

        private string HexToString(int hex)
        {
            int value = Convert.ToInt32(hex);
            string stringValue = Char.ConvertFromUtf32(value);
            return stringValue;
        }

        private bool IsRelayOn(byte[] responseBytes)
        {
            if (responseBytes != null && responseBytes.Length > 7)
            {
                var byteV = responseBytes[responseBytes.Length - 1];
                if (byteV == 0x00)
                {
                    return false;
                }
                else if (byteV == 0x01)
                {
                    return true;
                }
            }
            throw new Exception("could not determine lazy bone current state, because response was incomplete");
        }

        private static string GetLazyBoneSoftwareVersion(byte[] responseBytes)
        {
            if (responseBytes != null)
            {
                //Check if byte - 2 == 0x0f (15 dec)
                if (responseBytes[responseBytes.Length - 2] == 0x0f)
                {
                    //Get next byte (= software version)
                    var byteArr = new byte[1];
                    byteArr[0] = responseBytes[responseBytes.Length - 1];
                    return Encoding.ASCII.GetString(byteArr);
                }
            }
            return "";
        }
    }
}
