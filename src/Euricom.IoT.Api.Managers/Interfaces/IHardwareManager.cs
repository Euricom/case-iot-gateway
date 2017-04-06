using Euricom.IoT.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IHardwareManager
    {
        string GetDeviceName(string deviceId);
        IEnumerable<Device> GetHardwareDevices();
        Hardware GetHardware();
        Task<Device> AddHardware(Device device);
        Task<bool> DeleteHardware(string deviceid);
    }
}
