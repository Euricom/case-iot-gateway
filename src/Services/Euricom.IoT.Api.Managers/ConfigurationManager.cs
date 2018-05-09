using AutoMapper;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Models;
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

        public SettingsDto GetConfigSettings()
        {
            var settings = _settingsRepository.Get();

            return Mapper.Map<SettingsDto>(settings);
        }

        public void SaveConfigSettings(SettingsDto settingsDto)
        {
            var settings = Mapper.Map<Settings>(settingsDto);

            _settingsRepository.Update(settings);
        }
    }
}
