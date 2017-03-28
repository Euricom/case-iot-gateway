using Euricom.IoT.Devices.DanaLock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IDanaLockManager
    {
        bool IsLocked(string deviceId);
        Common.DanaLock Add(Common.DanaLock danaLock);
        void Switch(string deviceid, string state);
    }
}
