using Euricom.IoT.Api.Manager;
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

        public Device GetDevice(string deviceId)
        {
            return GetHardwareDevices().SingleOrDefault(x => x.DeviceId == deviceId);
        }

        public async Task<Device> AddHardware(Device device)
        {
            switch (device.Type)
            {
                case HardwareType.Camera:
                    device = await _cameraManager.Add(new Camera()
                    {
                        Name = device.Name
                    });
                    break;
                case HardwareType.DanaLock:
                    device = await _danaLockManager.Add(new Common.DanaLock()
                    {
                        Name = device.Name
                    });
                    break;
                case HardwareType.LazyBoneSwitch:
                    device = await _lazyBoneManager.Add(new Common.LazyBone()
                    {
                        Name = device.Name
                    });
                    break;
                default:
                    throw new ArgumentException($"type: {device.Type} has no implementation..");
            }
            return device;
        }

        public async Task<bool> DeleteHardware(string deviceId)
        {
            var device = GetDevice(deviceId);
            switch (device.Type)
            {
                case HardwareType.Camera:
                    return await _cameraManager.Remove(deviceId);
                case HardwareType.DanaLock:
                    return await _danaLockManager.Remove(deviceId);
                case HardwareType.LazyBoneSwitch:
                    return await _lazyBoneManager.Remove(deviceId);
                default:
                    throw new ArgumentException($"type: {device.Type} has no implementation..");
            }
        }
    }
}
