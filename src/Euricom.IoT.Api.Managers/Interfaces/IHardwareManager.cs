using Euricom.IoT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IHardwareManager
    {
        Task<Device> AddHardware(Device device);
        Task DeleteHardware(string deviceName);
        Device GetDeviceById(string deviceId);
        Device GetDeviceByName(string deviceName);
        string GetDeviceId(string deviceName);
        string GetDeviceName(string deviceId);
        Hardware GetHardware();
        IEnumerable<Device> GetHardwareDevices();
    }
}
