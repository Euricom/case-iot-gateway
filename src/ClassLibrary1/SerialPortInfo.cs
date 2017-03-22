//using Euricom.IoT.Api.OpenZWave;
using OpenZWave;
using Windows.Devices.Enumeration;

namespace Euricom.IoT.DanaLock.Core
{

    public sealed class SerialPortInfo
    {
#if NETFX_CORE
        internal SerialPortInfo(DeviceInformation info)
        {
            PortID = info.Id;
            Name = info.Name;
        }
#else
            internal SerialPortInfo(string id)
            {
                PortID = id;
                Name = id;
            }
#endif
        public string PortID { get; }
        public string Name { get; }
        private bool _isActive;

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
    }
}
