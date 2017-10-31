//using Euricom.IoT.Api.OpenZWave;
using OpenZWave;
using Windows.Devices.Enumeration;

namespace Euricom.IoT.ZWave
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
        public bool IsActive { get; private set; }

        public void Activate(ZWManager manager)
        {
            if (IsActive == false)
            {
                IsActive = ZWManager.Instance.AddDriver(PortID);
            }
        }

        public void Deactivate(ZWManager manager)
        {
            if (IsActive)
            {
                IsActive = !ZWManager.Instance.RemoveDriver(PortID);
            }
        }
    }
}
