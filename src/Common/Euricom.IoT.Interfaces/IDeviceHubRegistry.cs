using System.Threading.Tasks;
using Euricom.IoT.Models;

namespace Euricom.IoT.Interfaces
{
    public interface IDeviceHubRegistry
    {
        Task<string> AddDeviceAsync(string deviceId, HardwareType type);
        Task RemoveDeviceAsync(string deviceId);
    }
}