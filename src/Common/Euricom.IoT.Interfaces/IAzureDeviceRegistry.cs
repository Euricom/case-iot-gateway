using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IAzureDeviceRegistry
    {
        Task<string> AddDeviceAsync(string deviceId);
        Task RemoveDeviceAsync(string deviceId);
    }
}