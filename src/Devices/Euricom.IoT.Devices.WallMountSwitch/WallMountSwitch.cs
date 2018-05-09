using System;
using System.Collections.Generic;
using Euricom.IoT.Devices.ZWave;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Devices.WallMountSwitch
{
    public class WallMountSwitch : ZWaveDevice
    {
        // EF
        private WallMountSwitch() { }

        public WallMountSwitch(string deviceId, string primaryKey, byte nodeId, string name, bool enabled, int pollingTime)
            : base(deviceId, primaryKey, HardwareType.WallMountSwitch, nodeId)
        {
            Name = name;
            Enabled = enabled;
            PollingTime = pollingTime;
        }

        public int PollingTime { get; protected set; }

        public void Update(byte nodeId, string name, int pollingTime, bool enabled)
        {
            NodeId = nodeId;
            Name = name;
            PollingTime = pollingTime;
            Enabled = enabled;
        }

        #region Functionality
        public bool IsOn(IZWaveController controller)
        {
            EnforceEnabled();

            return controller.GetValue(NodeId, 37);
        }

        public void TurnOn(IZWaveController controller)
        {
            EnforceEnabled();

            controller.SetValue(NodeId, 37, true);
        }

        public void TurnOff(IZWaveController controller)
        {
            EnforceEnabled();

            controller.SetValue(NodeId, 37, false);
        }

        public bool TestConnection(IZWaveController controller)
        {
            EnforceEnabled();

            return controller.TestConnection(NodeId);
        }
        #endregion

        public bool On { get; private set; }

        public override bool UpdateState(byte key, byte value)
        {
            if (key != 37)
            {
                return false;
            }

            var val = Convert.ToBoolean(value);
            if (On == val)
            {
                return false;
            }

            On = val;
            return true;
        }

        public override Dictionary<string, object> GetState()
        {
            return new Dictionary<string, object>
            {
                { "State", On }
            };
        }
    }
}
