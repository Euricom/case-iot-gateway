using Euricom.IoT.Devices.ZWave;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Devices.WallMountSwitch
{
    public class WallMountSwitch : ZWaveDevice
    {
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
            return controller.GetValue(NodeId, 0x25);
        }

        public void TurnOn(IZWaveController controller)
        {
            controller.SetValue(NodeId, 0x25, true);
        }

        public void TurnOff(IZWaveController controller)
        {
            controller.SetValue(NodeId, 0x25, false);
        }

        public bool TestConnection(IZWaveController controller)
        {
            return controller.TestConnection(NodeId);
        }
        #endregion
    }
}
