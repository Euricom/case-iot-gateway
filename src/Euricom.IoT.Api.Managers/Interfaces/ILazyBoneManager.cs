using Euricom.IoT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ILazyBoneManager
    {
        Task<IEnumerable<Euricom.IoT.Models.LazyBone>> GetAll();
        Task<Euricom.IoT.Models.LazyBone> GetByDeviceId(string deviceId);
        Task<Euricom.IoT.Models.LazyBone> GetByDeviceName(string deviceName);
        Task<Euricom.IoT.Models.LazyBone> Add(Euricom.IoT.Models.LazyBone danaLock);
        Task<Euricom.IoT.Models.LazyBone> Edit(Euricom.IoT.Models.LazyBone danaLock);
        Task<bool> TestConnection(string deviceId);
        Task<bool> GetCurrentStateSwitch(string deviceId);
        Task<LazyBoneDimmerState> GetCurrentStateDimmer(string deviceId);
        Task Switch(string deviceId, string state);
        Task SetLightValue(string deviceId, int value);
        Task<bool> Remove(string devicename);
    }
}
