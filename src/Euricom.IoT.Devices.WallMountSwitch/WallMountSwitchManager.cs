using Euricom.IoT.ZWave;
using OpenZWave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.WallMountSwitch
{
    // This class uses the SwitchBinary implementation of OpenZWave
    // https://github.com/OpenZWave/open-zwave/blob/Dev/cpp/src/command_classes/SwitchBinary.h
    public class WallMountSwitchManager : IWallMountSwitchManager
    {
        public WallMountSwitchManager()
        {
        }

        public bool IsOn(byte nodeId)
        {
            uint homeId = GetHomeId();
            bool currentVal = false;
            ZWaveManager.Instance.ZWManager.GetValueAsBool(new ZWValueID(homeId, nodeId, ZWValueGenre.User, 0x25, 1, 0, ZWValueType.Bool, 0), out currentVal);
            return currentVal;
        }

        public void SetOn(byte nodeId)
        {
            uint homeId = GetHomeId();
            ZWManager.Instance.SetValue(new ZWValueID(homeId, nodeId, ZWValueGenre.User, 0x25, 1, 0, ZWValueType.Bool, 0), false);
        }

        public void SetOff(byte nodeId)
        {
            uint homeId = GetHomeId();
            ZWManager.Instance.SetValue(new ZWValueID(homeId, nodeId, ZWValueGenre.User, 0x25, 1, 0, ZWValueType.Bool, 0), true);
        }

        public bool TestConnection(byte nodeId)
        {
            return ZWaveManager.Instance.TestConnection(nodeId);
        }

        private static uint GetHomeId()
        {
            uint homeId = ZWaveManager.Instance.HomeId;
            if (homeId == 0)
                throw new InvalidOperationException("OpenZWave was not initialized correct. HomeId was 0");
            return homeId;
        }
    }
}
