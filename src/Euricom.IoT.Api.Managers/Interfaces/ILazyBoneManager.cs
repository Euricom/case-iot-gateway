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
        Task Remove(string devicename);
        Task<bool> TestConnection(string deviceId);
        Task<bool> GetCurrentStateSwitch(string deviceId);
        Task<LazyBoneDimmerState> GetCurrentStateDimmer(string deviceId);
        Task Switch(string deviceId, string state);

        /// <summary>
        /// Changes light intensity of specific device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="value">Must be between 1 and 255 inclusive (1 is brightest, 255 is darkest)</param>
        /// <returns></returns>
        Task SetLightValue(string deviceId, int value);

        /// <summary>
        /// Changes light intensity three times as a test
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task TestChangeLightIntensity(string deviceId);

    }
}
