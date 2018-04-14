using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IAzureDeviceRegistry _deviceRegistry;

        public ConfigurationManager(ISettingsRepository settingsRepository, IAzureDeviceRegistry deviceRegistry)
        {
            _settingsRepository = settingsRepository;
            _deviceRegistry = deviceRegistry;
        }

        public Settings GetConfigSettings()
        {
            return _settingsRepository.Get();
        }

        public void SaveConfigSettings(Settings settings)
        {
            _settingsRepository.Update(settings);
        }
    }
}
