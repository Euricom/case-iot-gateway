﻿using Euricom.IoT.Api.Manager;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class HardwareManager : IHardwareManager
    {
        private readonly ICameraManager _cameraManager;
        private readonly IDanaLockManager _danaLockManager;
        private readonly ILazyBoneManager _lazyBoneManager;


        public HardwareManager()
        {
            _cameraManager = new CameraManager();
            _danaLockManager = new DanaLockManager();
            _lazyBoneManager = new LazyBoneManager();
        }

        public string GetDeviceId(string deviceName)
        {
            var device = GetDeviceByName(deviceName);
            if (device == null)
            {
                throw new Exception($"Could not find deviceName: {deviceName}");
            }
            return device.DeviceId;

        }

        public string GetDeviceName(string deviceId)
        {
            var device = GetDeviceById(deviceId);
            if (device == null)
            {
                throw new Exception($"Could not find deviceId: {deviceId}");
            }
            return device.Name;
        }

        public IEnumerable<Device> GetHardwareDevices()
        {
            var devices = new List<Device>();
            var hardware = GetHardware();

            foreach (var hardwareItem in hardware.Cameras)
            {
                devices.Add(new Device()
                {
                    DeviceId = hardwareItem.DeviceId,
                    Name = hardwareItem.Name,
                    Type = HardwareType.Camera
                });
            }

            foreach (var hardwareItem in hardware.Switches)
            {
                devices.Add(new Device()
                {
                    DeviceId = hardwareItem.DeviceId,
                    Name = hardwareItem.Name,
                    Type = HardwareType.LazyBoneSwitch
                });
            }

            foreach (var hardwareItem in hardware.DanaLocks)
            {
                devices.Add(new Device()
                {
                    DeviceId = hardwareItem.DeviceId,
                    Name = hardwareItem.Name,
                    Type = HardwareType.DanaLock
                });
            }

            return devices;
        }

        public Hardware GetHardware()
        {
            var hardware = Database.Instance.GetHardware();
            return hardware;
        }

        public Device GetDeviceById(string deviceId)
        {
            return GetHardwareDevices().SingleOrDefault(x => x.DeviceId == deviceId);
        }

        public Device GetDeviceByName(string deviceName)
        {
            return GetHardwareDevices().SingleOrDefault(x => x.Name == deviceName);
        }

        public async Task<Device> AddHardware(Device device)
        {
            switch (device.Type)
            {
                case HardwareType.Camera:
                    device = await _cameraManager.Add(new Camera()
                    {
                        Name = device.Name,
                        Type = HardwareType.Camera
                    });
                    break;
                case HardwareType.DanaLock:
                    device = await _danaLockManager.Add(new Common.DanaLock()
                    {
                        Name = device.Name,
                        Type = HardwareType.DanaLock
                    });
                    break;
                case HardwareType.LazyBoneSwitch:
                    device = await _lazyBoneManager.Add(new Common.LazyBone()
                    {
                        Name = device.Name,
                        Type = HardwareType.LazyBoneSwitch
                    });
                    break;
                default:
                    throw new ArgumentException($"type: {device.Type} has no implementation..");
            }
            return device;
        }

        public async Task<bool> DeleteHardware(string deviceName)
        {
            var device = GetDeviceByName(deviceName);
            switch (device.Type)
            {
                case HardwareType.Camera:
                    return await _cameraManager.Remove(device.Name);
                case HardwareType.DanaLock:
                    return await _danaLockManager.Remove(device.Name);
                case HardwareType.LazyBoneSwitch:
                    return await _lazyBoneManager.Remove(device.Name);
                default:
                    throw new ArgumentException($"type: {device.Type} has no implementation..");
            }
        }
    }
}
