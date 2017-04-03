using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace Euricom.IoT.AzureDeviceManager
{
    public interface IAzureDeviceManager
    {
        Task<IEnumerable<Device>> GetDevicesAsync();
        Task<string> AddDeviceAsync(string deviceName);
        Task RemoveDeviceAsync(string deviceName);
    }
}