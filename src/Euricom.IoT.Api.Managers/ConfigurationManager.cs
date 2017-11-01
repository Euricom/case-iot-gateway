using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IUserRepository _userRepository;

        public ConfigurationManager(ISettingsRepository settingsRepository, IUserRepository userRepository)
        {
            _settingsRepository = settingsRepository;
            _userRepository = userRepository;
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
