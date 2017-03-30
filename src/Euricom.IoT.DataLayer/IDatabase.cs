using System.Collections.Generic;
using Euricom.IoT.Common;

namespace Euricom.IoT.DataLayer
{
    public interface IDatabase
    {
        Device FindDevice(string deviceid);
        bool RemoveDevice(string deviceid);
        Camera GetCameraConfig(string deviceId);
        List<Camera> GetCameras();
        DanaLock GetDanaLockConfig(string deviceId);
        List<DanaLock> GetDanaLocks();
        Hardware GetHardware();
        LazyBone GetLazyBoneConfig(string deviceId);
        List<LazyBone> GetLazyBones();
        Log GetLog();
        void SetValue(string table, string key, string value);
    }
}