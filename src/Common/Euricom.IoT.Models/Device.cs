using System;
using Euricom.IoT.Common;

namespace Euricom.IoT.Models
{
    public class Device
    {
        // EF
        protected Device() { }

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
        public string PrimaryKey { get; private set; }
        public HardwareType Type { get; private set; }
        public string Name { get; protected set; }
        public bool Enabled { get; protected set; }

        protected void EnforceEnabled()
        {
            if (Enabled == false)
            {
                throw new InvalidOperationException($"{Type} {DeviceId} is not enabled.");
            }
        }
    }
}
