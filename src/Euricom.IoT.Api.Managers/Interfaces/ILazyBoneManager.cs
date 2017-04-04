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
        Task<Common.LazyBone> Get(string deviceId);
        Task<Common.LazyBone> Add(Common.LazyBone danaLock);
        Task<Common.LazyBone> Edit(Common.LazyBone danaLock);
        Task<bool> TestConnection(string deviceId);
        Task Switch(string device, string state);
        Task<bool> Remove(string deviceId);
    }
}
