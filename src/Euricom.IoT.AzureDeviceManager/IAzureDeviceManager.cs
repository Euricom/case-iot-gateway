using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace Euricom.IoT.AzureDeviceManager
{
    public interface IAzureDeviceManager
    {
        Task<IEnumerable<Device>> GetDevicesAsync();
        Task<string> AddDeviceAsync(string deviceId);
        Task RemoveDeviceAsync(string deviceId);
        void UpdateConnectionString(string connectionstring);
    }
}