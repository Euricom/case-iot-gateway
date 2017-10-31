using Euricom.IoT.ZWave;
using OpenZWave;
using System;
using Euricom.IoT.ZWave.Interfaces;

namespace Euricom.IoT.DanaLock
{
    public class DanaLockManager : IDanaLockManager
    {
        private readonly IZWaveManager _zWaveManager;

        public DanaLockManager(IZWaveManager zWaveManager)
        {
            _zWaveManager = zWaveManager;
        }

        public bool IsLocked(byte nodeId)
        {
            uint homeId = GetHomeId();
            bool currentVal = false;
            ZWaveManager.ZWManager.GetValueAsBool(new ZWValueID(homeId, nodeId, ZWValueGenre.User, 0x62, 1, 0, ZWValueType.Bool, 0), out currentVal);
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

        private uint GetHomeId()
        {
            uint homeId = _zWaveManager.HomeId;
            if (homeId == 0)
                throw new InvalidOperationException("OpenZWave was not initialized correct. HomeId was 0");
            return homeId;
        }

        public bool TestConnection(byte nodeId)
        {
            return _zWaveManager.TestConnection(nodeId);
        }
    }
}
