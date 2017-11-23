using Euricom.IoT.Models;
using Euricom.IoT.ZWave.Interfaces;

namespace Euricom.IoT.Devices.WallMountSwitch
{
    public class WallMountSwitch : ZWaveDevice
    {
        public WallMountSwitch(string deviceId, byte nodeId, string name, bool enabled, int pollingTime)
            : base(deviceId, HardwareType.WallMountSwitch, nodeId)
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
        public bool IsOn(IZWaveManager manager)
        {
            return manager.GetValue(NodeId, 0x25);
        }

        public void TurnOn(IZWaveManager manager)
        {
            manager.SetValue(NodeId, 0x25, true);
        }

        public void TurnOff(IZWaveManager manager)
        {
            manager.SetValue(NodeId, 0x25, false);
        }

        public bool TestConnection(IZWaveManager manager)
        {
            return manager.TestConnection(NodeId);
        }
        #endregion
    }
}
