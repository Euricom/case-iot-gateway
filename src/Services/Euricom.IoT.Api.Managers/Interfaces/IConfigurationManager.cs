using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IConfigurationManager
    {
        SettingsDto GetConfigSettings();
        void SaveConfigSettings(SettingsDto settingsDto);
    }
}
