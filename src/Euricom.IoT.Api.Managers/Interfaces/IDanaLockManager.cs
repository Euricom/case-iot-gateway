using System.Collections.Generic;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IDanaLockManager
    {
        IEnumerable<DanaLockDto> Get();
        DanaLockDto Get(string deviceId);
        DanaLockDto Add(DanaLockDto danalock);
        DanaLockDto Update(DanaLockDto danalock);
        void Remove(string deviceId);

        bool TestConnection(string deviceId);
        bool IsLocked(string deviceId);
        void Switch(string deviceId, string state);
    }
}
