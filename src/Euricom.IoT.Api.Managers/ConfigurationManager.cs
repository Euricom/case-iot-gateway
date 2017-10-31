using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly ISettingsRepository _settingsRepository;

        public ConfigurationManager(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public Settings GetConfigSettings()
        {
            var settings = _settingsRepository.Get();
            settings.Password = string.Empty;
            return settings;
        }

        public void SaveConfigSettings(Settings settings)
        {
            _settingsRepository.Update(settings);
        }
    }
}
