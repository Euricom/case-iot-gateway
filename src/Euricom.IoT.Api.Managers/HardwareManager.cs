using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer;
using System;
using System.Collections.Generic;

namespace Euricom.IoT.Api.Managers
{
    public class HardwareManager : IHardwareManager
    {
        private readonly ICameraManager _cameraManager;
        private readonly IDanaLockManager _danaLockManager;
        private readonly ILazyBoneManager _lazyBoneManager;


        public HardwareManager()
        {

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

        public Device AddHardware(Device device)
        {
            switch (device.Type)
            {
                case HardwareType.Camera:
                    device = _cameraManager.Add(new Camera()
                    {
                        Name = device.Name
                    });
                    break;
                case HardwareType.DanaLock:
                    device = _danaLockManager.Add(new Common.DanaLock()
                    {
                        Name = device.Name
                    });
                    break;
                case HardwareType.LazyBoneSwitch:
                    device = _lazyBoneManager.Add(new Common.LazyBone()
                    {
                        Name = device.Name
                    });
                    break;
                default:
                    throw new ArgumentException($"type: {device.Type} has no implementation..");
            }
            return device;
        }

        public bool DeleteHardware(string deviceid)
        {
            return Database.Instance.RemoveDevice(deviceid);
        }
    }
}
