using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IConfigurationManager
    {
        Settings GetConfigSettings();
        void SaveConfigSettings(Settings settings);
    }
}
