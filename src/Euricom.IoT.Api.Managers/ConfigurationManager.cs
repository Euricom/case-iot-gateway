using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class ConfigurationManager : IConfigurationManager
    {
        public ConfigurationManager()
        {
        }

        public Settings GetConfigSettings()
        {
            var settings = Database.Instance.GetConfigSettings();
            settings.Password = string.Empty;
            return settings;
        }

        public void SaveConfigSettings(Settings settings)
        {
            Database.Instance.SaveConfigSettings(settings);
        }
    }
}
