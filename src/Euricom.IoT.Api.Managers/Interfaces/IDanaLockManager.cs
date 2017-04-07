using Euricom.IoT.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IDanaLockManager
    {
        Task<IEnumerable<Common.DanaLock>> GetAll();
        Task<DanaLock> GetByDeviceId(string deviceId);
        Task<DanaLock> GetByDeviceName(string deviceName);
        Task<Common.DanaLock> Add(Common.DanaLock danaLock);
        Task<Common.DanaLock> Edit(Common.DanaLock danaLock);

        Task<bool> TestConnection(string deviceid);
        Task<bool> IsLocked(string deviceId);
        Task Switch(string deviceid, string state);
        Task<bool> Remove(string devicename);
    }
}
