﻿using Euricom.IoT.Models;
using System.Collections.Generic;

namespace Euricom.IoT.DataLayer
{
    public interface IDatabase
    {
        bool RemoveDevice(string deviceid);
        Camera GetCameraConfig(string deviceId);
        List<Camera> GetCameras();
        LazyBone GetLazyBoneConfig(string deviceId);
        List<LazyBone> GetLazyBones();
        void SetValue(string table, string key, string value);
    }
}