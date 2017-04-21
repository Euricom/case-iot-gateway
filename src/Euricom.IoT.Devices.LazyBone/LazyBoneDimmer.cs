using Euricom.IoT.Common.Utilities;
using Euricom.IoT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.LazyBone
{
    public sealed class LazyBoneDimmer : LazyBone
    {
        // Get lamp states - sends 2 bytes back to the controller: 'on/off state'+'light value'  
        // on/off state: 0-OFF, 1-ON
        // light value:  0x00-0xFF,   0x00- Brightest, 0xFF- Darkest
        private readonly byte[] COMMAND_GET_LAMP_STATES = new byte[] { 0x5B, 0x00, 0x0D };

        // 65 00 0D
        private readonly byte[] COMMAND_TURN_ON_LAMP = new byte[] { 0x65, 0x00, 0x0D };

        // 6F 00 0D
        private readonly byte[] COMMAND_TURN_OFF_LAMP = new byte[] { 0x6F, 0x00, 0x0D };

        private SocketClient _client;

        public LazyBoneDimmer(SocketClient socketClient)
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

        public async Task<LazyBoneDimmerState> GetCurrentState()
        {
            lock (_client)
            {
                var response = _client.Send(HexUtilities.ToHexString(COMMAND_GET_LAMP_STATES), true).Result;
                return GetCurrentState(response);
            }
        }

        public async Task SetLightValue(short? value)
        {
            if (!value.HasValue)
                return;

            if (value < 0x00)
                throw new ArgumentOutOfRangeException("value must not be less than 0");

            if (value > 0xff)
                throw new ArgumentOutOfRangeException("value must be less than 255");

            lock (_client)
            {
                var cmdStr = HexUtilities.ToHexString(0x67);
                cmdStr += HexUtilities.ToHexString(value.Value);
                cmdStr += HexUtilities.ToHexString(0x0D);
                var response = _client.Send(cmdStr, false).Result;
            }
        }

        public async Task SetLightOn()
        {
            lock (_client)
            {
                var t = _client.Send(HexUtilities.ToHexString(COMMAND_TURN_ON_LAMP), false).Result;
            }
        }

        public async Task SetLightOff()
        {
            lock (_client)
            {
                var t = _client.Send(HexUtilities.ToHexString(COMMAND_TURN_OFF_LAMP), false).Result;
            }
        }

        private LazyBoneDimmerState GetCurrentState(byte[] response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (response.Length != 2)
                throw new ArgumentNullException("response was not complete or unexpected response length");

            // First byte is OFF or ON (OFF = 0x00 , ON = 0x01)
            var lightOn = response[0] == 0x01;

            // Second byte is light value beteween 0x00 and 0xFF
            // 0x00- Brightest, 0xFF- Darkest
            var lightValue = response[1];

            return new LazyBoneDimmerState()
            {
                LightOn = lightOn,
                LightValue = (int)lightValue
            };
        }
    }
}
