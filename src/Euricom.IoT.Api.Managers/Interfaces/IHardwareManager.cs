using Euricom.IoT.Common;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IHardwareManager
    {
        Hardware GetHardware();
        void AddHardware(string name, string type);
    }
}
