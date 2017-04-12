using Euricom.IoT.Models;
using System.Collections.Generic;

namespace Euricom.IoT.DataLayer
{
    public interface IDatabase
    {
        bool ExistsUser(string username);
        void AddUser(string username, string password);
        bool CheckUser(string username, string password);
        bool EditPassword(string username, string password);
        void RemoveUser(string username);

        Device FindDevice(string deviceid);
        bool RemoveDevice(string deviceid);
        Camera GetCameraConfig(string deviceId);
        List<Camera> GetCameras();
        DanaLock GetDanaLockConfig(string deviceId);
        List<DanaLock> GetDanaLocks();
        Hardware GetHardware();
        LazyBone GetLazyBoneConfig(string deviceId);
        List<LazyBone> GetLazyBones();
        void SetValue(string table, string key, string value);
    }
}