using Euricom.IoT.Api.Manager;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Models;
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
        private readonly IWallMountSwitchManager _wallmountManager;

        public HardwareManager()
        {
            _cameraManager = new CameraManager();
            _danaLockManager = new DanaLockManager();
            _lazyBoneManager = new LazyBoneManager();
            _wallmountManager = new WallMountSwitchManager();
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

            foreach (var hardwareItem in hardware.LazyBones)
            {
                devices.Add(new Device()
                {
                    DeviceId = hardwareItem.DeviceId,
                    Name = hardwareItem.Name,
                    Type = hardwareItem.IsDimmer == false ? HardwareType.LazyBoneSwitch : HardwareType.LazyBoneDimmer
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

            foreach (var hardwareItem in hardware.WallMountSwitches)
            {
                devices.Add(new Device()
                {
                    DeviceId = hardwareItem.DeviceId,
                    Name = hardwareItem.Name,
                    Type = HardwareType.WallmountSwitch
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
                    device = await _danaLockManager.Add(new Euricom.IoT.Models.DanaLock()
                    {
                        Name = device.Name,
                        Type = HardwareType.DanaLock
                    });
                    break;
                case HardwareType.LazyBoneSwitch:
                    device = await _lazyBoneManager.Add(new Euricom.IoT.Models.LazyBone()
                    {
                        Name = device.Name,
                        Type = HardwareType.LazyBoneSwitch
                    });
                    break;
                case HardwareType.LazyBoneDimmer:
                    device = await _lazyBoneManager.Add(new Euricom.IoT.Models.LazyBone()
                    {
                        Name = device.Name,
                        Type = HardwareType.LazyBoneDimmer
                    });
                    break;
                case HardwareType.WallmountSwitch:
                    device = await _wallmountManager.Add(new Euricom.IoT.Models.WallMountSwitch()
                    {
                        Name = device.Name,
                        Type = HardwareType.WallmountSwitch
                    });
                    break;
                default:
                    throw new ArgumentException($"type: {device.Type} has no implementation..");
            }
            return device;
        }

        public async Task DeleteHardware(string deviceName)
        {
            var device = GetDeviceByName(deviceName);
            switch (device.Type)
            {
                case HardwareType.Camera:
                    await _cameraManager.Remove(device.Name);
                    break;
                case HardwareType.DanaLock:
                    await _danaLockManager.Remove(device.Name);
                    break;
                case HardwareType.LazyBoneSwitch:
                    await _lazyBoneManager.Remove(device.Name);
                    break;
                default:
                    throw new ArgumentException($"type: {device.Type} has no implementation..");
            }
        }
    }
}
