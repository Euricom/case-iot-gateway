using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ILazyBoneManager
    {
        Task<IEnumerable<Common.LazyBone>> GetAll();
        Task<Common.LazyBone> GetByDeviceId(string deviceId);
        Task<Common.LazyBone> GetByDeviceName(string deviceName);
        Task<Common.LazyBone> Add(Common.LazyBone danaLock);
        Task<Common.LazyBone> Edit(Common.LazyBone danaLock);
        Task<string> TestConnection(string deviceId);
        Task<bool> GetCurrentState(string deviceId);
        Task Switch(string deviceId, string state);
        Task<bool> Remove(string devicename);
    }
}
