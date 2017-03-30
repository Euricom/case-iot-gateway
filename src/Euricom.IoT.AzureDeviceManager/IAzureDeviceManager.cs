using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace Euricom.IoT.AzureDeviceManager
{
    public interface IAzureDeviceManager
    {
        Task<string> AddDeviceAsync(string deviceName);
        Task<IEnumerable<Device>> GetDevicesAsync();
    }
}