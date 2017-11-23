using System;
using Euricom.IoT.Common;

namespace Euricom.IoT.Models
{
    public class Device
    {
        public Device(string deviceId, HardwareType type)
        {
            if (Validation.ValidateDeviceId(deviceId))
            {
                throw new ArgumentException(nameof(deviceId));
            }

            DeviceId = deviceId;
            Type = type;
        }

        public string DeviceId { get; private set; }
        public HardwareType Type { get; private set; }
        public string Name { get; protected set; }
        public bool Enabled { get; protected set; }
    }
}
