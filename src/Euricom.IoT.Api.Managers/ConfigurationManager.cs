using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IAzureDeviceManager _deviceManager;

        public ConfigurationManager(ISettingsRepository settingsRepository, IAzureDeviceManager deviceManager)
        {
            _settingsRepository = settingsRepository;
            _deviceManager = deviceManager;
        }

        public Settings GetConfigSettings()
        {
            return _settingsRepository.Get();
        }

        public void SaveConfigSettings(Settings settings)
        {
            _settingsRepository.Update(settings);

            _deviceManager.UpdateConnectionString(settings.AzureIotHubUriConnectionString);
        }
    }
}
