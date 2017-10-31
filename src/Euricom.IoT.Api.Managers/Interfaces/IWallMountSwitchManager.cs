using System.Collections.Generic;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IWallMountSwitchManager
    {
        IEnumerable<WallMountSwitchDto> Get();
        WallMountSwitchDto Get(string deviceId);
        WallMountSwitchDto Add(WallMountSwitchDto wallmount);
        WallMountSwitchDto Update(WallMountSwitchDto wallmount);
        void Remove(string deviceId);

        bool IsOn(string deviceId);
        void Switch(string deviceId, string state);
        bool TestConnection(string deviceId);
    }
}