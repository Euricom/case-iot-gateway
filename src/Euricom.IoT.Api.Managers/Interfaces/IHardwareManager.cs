using Euricom.IoT.Common;
using System.Collections.Generic;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IHardwareManager
    {
        IEnumerable<Device> GetHardwareDevices();
        Hardware GetHardware();
        Device AddHardware(Device device);
        bool DeleteHardware(string deviceid);
    }
}
