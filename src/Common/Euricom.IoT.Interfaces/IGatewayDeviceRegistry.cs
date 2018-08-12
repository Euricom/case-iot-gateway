using System.Collections.Generic;
using System.Threading.Tasks;
using Euricom.IoT.Models.Messages;

namespace Euricom.IoT.Interfaces
{
    public interface IGatewayDeviceRegistry
    {
        Task Initialize(Dictionary<string, string> devices);
        Task AddDeviceAsync(string deviceId, string primaryKey);
        Task RemoveDeviceAsync(string deviceId);
        Task UpdateStateAsync(string deviceId, Dictionary<string, object> properties);
        Task SendMessageAsync(string deviceId, DeviceMessage message);
    }
}