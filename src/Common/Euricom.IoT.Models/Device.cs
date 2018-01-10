using System;
using Euricom.IoT.Common;

namespace Euricom.IoT.Models
{
    public class Device
    {
        public Device(string deviceId, string primaryKey, HardwareType type)
        {
            if (Validation.ValidateDeviceId(deviceId))
            {
                throw new ArgumentException(nameof(deviceId));
            }

            DeviceId = deviceId;
            PrimaryKey = primaryKey;
            Type = type;
        }

        public string DeviceId { get; private set; }
        public string PrimaryKey { get; }
        public HardwareType Type { get; private set; }
        public string Name { get; protected set; }
        public bool Enabled { get; protected set; }
    }
}
