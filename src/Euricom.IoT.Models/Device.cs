using System;

namespace Euricom.IoT.Models
{
    public class Device
    {
        public Device(HardwareType type)
        {
            DeviceId = Guid.NewGuid().ToString("N");
            Type = type;
        }

        public string DeviceId { get; private set; }
        public HardwareType Type { get; private set; }
        public string Name { get; protected set; }
        public bool Enabled { get; protected set; }
    }
}
