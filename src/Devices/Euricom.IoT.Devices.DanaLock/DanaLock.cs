using Euricom.IoT.Devices.ZWave;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Devices.DanaLock
{
    public class DanaLock : ZWaveDevice
    {
        public DanaLock(string deviceId, string primaryKey, byte nodeId, string name, bool enabled, int pollingTime) 
            : base(deviceId, primaryKey, HardwareType.DanaLock, nodeId)
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

        public bool IsLocked(IZWaveController controller)
        {
            return controller.GetValue(NodeId, 0x62);
        }

        public void OpenLock(IZWaveController controller)
        {
            controller.SetValue(NodeId, 0x62, false);
        }

        public void CloseLock(IZWaveController controller)
        {
            controller.SetValue(NodeId, 0x62, true);
        }

        public bool TestConnection(IZWaveController controller)
        {
            return controller.TestConnection(NodeId);
        }

        #endregion
    }
}
