using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly Database _database;

        public ConfigurationManager(Database database)
        {
            _database = database;
        }

        public Settings GetConfigSettings()
        {
            var settings = _database.GetConfigSettings();
            settings.Password = string.Empty;
            return settings;
        }

        public void SaveConfigSettings(Settings settings)
        {
            _database.SaveConfigSettings(settings);
        }
    }
}
