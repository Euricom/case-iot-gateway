using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace AzureDeviceManager
{
    public interface IDeviceManager
    {
        Task<string> AddDeviceAsync(string deviceName);
        Task<IEnumerable<Device>> GetDevicesAsync();
    }
}