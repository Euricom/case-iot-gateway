using System.Collections.Generic;
using System.Threading.Tasks;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IWallMountSwitchManager
    {
        Task<Euricom.IoT.Models.WallMountSwitch> Add(Euricom.IoT.Models.WallMountSwitch wallmount);
        Task<Euricom.IoT.Models.WallMountSwitch> Edit(Euricom.IoT.Models.WallMountSwitch wallmount);
        Task<bool> Remove(string deviceName);
        Task<IEnumerable<Euricom.IoT.Models.WallMountSwitch>> GetAll();
        Task<Euricom.IoT.Models.WallMountSwitch> GetByDeviceId(string deviceId);
        Task<Euricom.IoT.Models.WallMountSwitch> GetByDeviceName(string deviceName);
        Task<bool> IsOn(byte nodeId);
        Task<bool> IsOn(string deviceId);
        Task Switch(string deviceId, string state);
        bool TestConnection(string deviceId);
    }
}