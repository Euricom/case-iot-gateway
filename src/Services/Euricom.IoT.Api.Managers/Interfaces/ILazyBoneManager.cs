using System.Collections.Generic;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Devices.LazyBone;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface ILazyBoneManager
    {
        IEnumerable<LazyBoneDto> Get();
        LazyBoneDto Get(string deviceId);
        Task<LazyBoneDto> Add(LazyBoneDto lazybone);
        LazyBoneDto Update(LazyBoneDto lazybone);
        Task Remove(string deviceId);

        bool TestConnection(string deviceId);
        /// <param name="deviceId"></param>
        /// <param name="state">Between 0 and 255</param>
        /// <returns></returns>
        void SetState(string deviceId, LazyBoneState state);
        /// <param name="deviceId"></param>
        /// <returns>Between 0 and 255</returns>
        LazyBoneState GetState(string deviceId);
    }
}
