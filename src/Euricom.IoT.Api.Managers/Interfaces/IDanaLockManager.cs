using Euricom.IoT.Common;
using Euricom.IoT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IDanaLockManager
    {
        Task<IEnumerable<Euricom.IoT.Models.DanaLock>> GetAll();
        Task<DanaLock> GetByDeviceId(string deviceId);
        Task<DanaLock> GetByDeviceName(string deviceName);
        Task<Euricom.IoT.Models.DanaLock> Add(Euricom.IoT.Models.DanaLock danaLock);
        Task<Euricom.IoT.Models.DanaLock> Edit(Euricom.IoT.Models.DanaLock danaLock);

        Task<bool> TestConnection(string deviceid);
        Task<bool> IsLocked(string deviceId);
        Task Switch(string deviceid, string state);
        Task<bool> Remove(string devicename);
    }
}
