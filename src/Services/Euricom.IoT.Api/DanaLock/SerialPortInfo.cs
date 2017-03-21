using OpenZWave;
using Windows.Devices.Enumeration;

namespace Euricom.IoT.Api
{
    public sealed class SerialPortInfo
    {
#if NETFX_CORE

        private bool _isActive;

        public SerialPortInfo(DeviceInformation info)
        {
            PortID = info.Id;
            Name = info.Name;
        }

#else
            public SerialPortInfo(string id)
            {
                PortID = id;
                Name = id;
            }
#endif

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (value)
                    ZWManager.Instance.AddDriver(PortID);
                else
                    ZWManager.Instance.RemoveDriver(PortID);
            }
        }

        public string Name { get; }
        public string PortID { get; }
    }
}
