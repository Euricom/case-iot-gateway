using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IGatewayDeviceRegistry
    {
        Task Initialize(Dictionary<string, string> devices);
        Task AddDeviceAsync(string deviceId, string primaryKey);
        Task RemoveDeviceAsync(string deviceId);
        Task SendAsync(string deviceId, Dictionary<string, object> properties);
    }
}