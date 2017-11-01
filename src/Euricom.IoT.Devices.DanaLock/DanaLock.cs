using Euricom.IoT.Models;
using Euricom.IoT.ZWave.Interfaces;

namespace Euricom.IoT.Devices.DanaLock
{
    public class DanaLock : ZWaveDevice
    {
        public DanaLock(byte nodeId, string name, bool enabled, int pollingTime) : base(HardwareType.DanaLock, nodeId)
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

        public bool IsLocked(IZWaveManager manager)
        {
            return manager.GetValue(NodeId, 0x62);
        }

        public void OpenLock(IZWaveManager manager)
        {
            manager.SetValue(NodeId, 0x62, false);
        }

        public void CloseLock(IZWaveManager manager)
        {
            manager.SetValue(NodeId, 0x62, true);
        }
        public bool TestConnection(IZWaveManager manager)
        {
            return manager.TestConnection(NodeId);
        }

        #endregion
    }
}
