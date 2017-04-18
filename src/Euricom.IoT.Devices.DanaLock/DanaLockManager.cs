using Euricom.IoT.ZWave;
using OpenZWave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.DanaLock
{
    public class DanaLockManager : IDanaLockManager
    {
        public DanaLockManager()
        {
        }

        public bool IsLocked(byte nodeId)
        {
            uint homeId = GetHomeId();
            bool currentVal = false;
            ZWaveManager.Instance.ZWManager.GetValueAsBool(new ZWValueID(homeId, nodeId, ZWValueGenre.User, 0x62, 1, 0, ZWValueType.Bool, 0), out currentVal);
            return currentVal;
        }

        public void OpenLock(byte nodeId)
        {
            uint homeId = GetHomeId();
            // Unlock or lock door
            ZWManager.Instance.SetValue(new ZWValueID(homeId, nodeId, ZWValueGenre.User, 0x62, 1, 0, ZWValueType.Bool, 0), false);
        }

        public void CloseLock(byte nodeId)
        {
            uint homeId = GetHomeId();
            //Unlock or lock door
            ZWManager.Instance.SetValue(new ZWValueID(homeId, nodeId, ZWValueGenre.User, 0x62, 1, 0, ZWValueType.Bool, 0), true);
        }

        private static uint GetHomeId()
        {
            uint homeId = ZWaveManager.Instance.HomeId;
            if (homeId == 0)
                throw new InvalidOperationException("OpenZWave was not initialized correct. HomeId was 0");
            return homeId;
        }

        public bool TestConnection(byte nodeId)
        {
            return ZWaveManager.Instance.TestConnection(nodeId);
        }
    }
}
