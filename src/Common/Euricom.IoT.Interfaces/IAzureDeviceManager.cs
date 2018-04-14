using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IAzureDeviceManager
    {
        Task UpdateStateAsync(string deviceId, string primaryKey, Dictionary<string, object> properties);
    }
}