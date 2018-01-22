using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IAzureDeviceManager
    {
        Task<string> AddDeviceAsync(string deviceId);
        Task RemoveDeviceAsync(string deviceId);
        Task UpdateStateAsync(string deviceId, Dictionary<string, object> properties);
    }
}