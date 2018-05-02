using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IDeviceHubRegistry
    {
        Task<string> AddDeviceAsync(string deviceId);
        Task RemoveDeviceAsync(string deviceId);
    }
}