using System;
using System.Collections.Generic;
using Euricom.IoT.Devices.ZWave;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Devices.DanaLock
{
    public class DanaLock : ZWaveDevice
    {
        // EF
        private DanaLock() { }

        public DanaLock(string deviceId, string primaryKey, byte nodeId, string name, bool enabled, int pollingTime) 
            : base(deviceId, primaryKey, HardwareType.DanaLock, nodeId)
        {
            Name = name;
            Enabled = enabled;
            PollingTime = pollingTime;
            Locked = false;
        }

        public int PollingTime { get; protected set; }

        public bool Locked { get; private set; }

        public void Update(byte nodeId, string name, int pollingTime, bool enabled)
        {
            NodeId = nodeId;
            Name = name;
            PollingTime = pollingTime;
            Enabled = enabled;
        }

        #region Functionality

        public bool IsLocked(IZWaveController controller)
        {
            Locked = controller.GetValue(NodeId, 0x62);

            return Locked;
        }

        public void OpenLock(IZWaveController controller)
        {
            controller.SetValue(NodeId, 98, false);
        }

        public void CloseLock(IZWaveController controller)
        {
            controller.SetValue(NodeId, 98, true);
        }

        public bool TestConnection(IZWaveController controller)
        {
            return controller.TestConnection(NodeId);
        }

        #endregion
        
        public override bool UpdateState(byte key, byte value)
        {
            if (key != 98)
            {
                return false;
            }

            var val = Convert.ToBoolean(value);
            if (Locked == val)
            {
                return false;
            }

            Locked = val;
            return true;
        }

        public override Dictionary<string, object> GetState()
        {
            return new Dictionary<string, object>
            {
                { "Locked", Locked }
            };
        }
    }
}
