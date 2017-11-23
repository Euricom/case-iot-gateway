using System;
using System.Diagnostics;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.Models;

namespace Euricom.IoT.Devices.LazyBone
{
    public class LazyBone: Device
    {
        public LazyBone(string deviceId, bool isDimmer, string name, bool enabled, int pollingTime, string host, short port)
            : base(deviceId, isDimmer == false
                ? HardwareType.LazyBoneSwitch
                : HardwareType.LazyBoneDimmer)
        {
            IsDimmer = isDimmer;
            Name = name;
            Enabled = enabled;
            PollingTime = pollingTime;
            Host = host;
            Port = port;
        }

        public string Host { get; set; }
        public short Port { get; set; }
        public int PollingTime { get; set; }
        public bool IsDimmer { get; set; }

        public void Update(string name, bool enabled, int pollingTime, string host, short port)
        {
            Name = name;
            Enabled = enabled;
            PollingTime = pollingTime;
            Host = host;
            Port = port;
        }

        public bool TestConnection(SocketClient client)
        {
            return client.TestConnection();
        }

        public LazyBoneState GetState(SocketClient client)
        {
            var response = client.Send(HexUtilities.ToHexString(LazyBoneStates.CommandGetLampStates), true);

            return GetCurrentState(response);
        }

        public void SetState(SocketClient client, LazyBoneState state)
        {
            if (state.On)
            {
                string message = HexUtilities.ToHexString(LazyBoneStates.CommandTurnOnLamp);
                var response = client.Send(message, false);

                Debug.WriteLine(response);

                if (IsDimmer && state.Intensity.HasValue)
                {
                    if (state.Intensity < 0x00)
                        throw new ArgumentOutOfRangeException(nameof(state), "value must not be less than 0");

                    if (state.Intensity > 0xff)
                        throw new ArgumentOutOfRangeException(nameof(state), "value must be less than 255");

                    var cmdStr = HexUtilities.ToHexString(0x67) + HexUtilities.ToHexString(state.Intensity.Value) +
                                 HexUtilities.ToHexString(0x0D);
                    response = client.Send(cmdStr, false);

                    Debug.WriteLine(response);
                }
            }
            else
            {
                string message = HexUtilities.ToHexString(LazyBoneStates.CommandTurnOffLamp);
                var response = client.Send(message, false);

                Debug.WriteLine(response);
            }
        }

        private LazyBoneState GetCurrentState(byte[] response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.Length != 2)
                throw new ArgumentException("Response was not complete or unexpected response length.");

            // First byte is OFF or ON (OFF = 0x00 , ON = 0x01)
            var lightOn = response[0] == 0x01;

            // Second byte is light value beteween 0x00 and 0xFF
            // 0x00- Brightest, 0xFF- Darkest
            var lightValue = response[1];

            return new LazyBoneState
            {
                On = lightOn,
                Intensity = lightValue
            };
        }
    }
}
