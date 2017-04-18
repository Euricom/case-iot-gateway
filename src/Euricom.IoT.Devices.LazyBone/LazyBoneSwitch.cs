using Euricom.IoT.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.LazyBone
{
    public sealed class LazyBoneSwitch : LazyBone
    {
        private readonly int COMMAND_GET_SOFTWARE_VERSION = 0x5A;
        private readonly int COMMAND_RELAY_TO_1 = 0x65;
        private readonly int COMMAND_RELAY_TO_0 = 0x6F;
        private readonly int COMMAND_RELAY_STATUS = 0x5B;

        private SocketClient _client;

        public LazyBoneSwitch(SocketClient socketClient)
        {
            _client = socketClient;
        }

        public async Task<bool> TestConnection()
        {
            lock (_client)
            {
                return _client.TestConnection().Result;
            }
        }

        public async Task<bool> GetCurrentState()
        {
            lock (_client)
            {
                var response = _client.Send(HexUtilities.ToHexString(COMMAND_RELAY_STATUS), true).Result;
                return IsRelayOn(response);
            }
        }

        public async Task Switch(bool on)
        {
            lock (_client)
            {
                string message = HexUtilities.ToHexString(on == true ? COMMAND_RELAY_TO_1 : COMMAND_RELAY_TO_0);
                var response = _client.Send(message, false).Result;
                Debug.WriteLine(response);
            }
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
