using System;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Logging;

namespace Euricom.IoT.DataLayer
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly IDbBreezeDatabase _database;

        public SettingsRepository(IDbBreezeDatabase database)
        {
            _database = database;
        }

        public Settings Get()
        {
            var settings = new Settings
            {
                HistoryLog = _database.GetValue<int>(Constants.DBREEZE_TABLE_SETTINGS, "HistoryLog"),
                GatewayDeviceKey = _database.GetValue<string>(Constants.DBREEZE_TABLE_SETTINGS, "GatewayDeviceKey"),
                AzureIotHubUri = _database.GetValue<string>(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUri"),
                AzureIotHubUriConnectionString = _database.GetValue<string>(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUriConnectionString"),
                AzureAccountName = _database.GetValue<string>(Constants.DBREEZE_TABLE_SETTINGS, "AzureAccountName"),
                AzureStorageAccessKey = _database.GetValue<string>(Constants.DBREEZE_TABLE_SETTINGS, "AzureStorageAccessKey"),
                DropboxAccessToken = _database.GetValue<string>(Constants.DBREEZE_TABLE_SETTINGS, "DropboxAccessToken")
            };

            var level = _database.GetValue<string>(Constants.DBREEZE_TABLE_SETTINGS, "LogLevel");
            if (string.IsNullOrEmpty(level))
            {
                settings.LogLevel = LogLevel.Information;
            }
            else
            {
                settings.LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), level);
            }

            return settings;
        }

        public void Update(Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _database.SetValue(Constants.DBREEZE_TABLE_SETTINGS, "HistoryLog", settings.HistoryLog.ToString());
            _database.SetValue(Constants.DBREEZE_TABLE_SETTINGS, "LogLevel", settings.LogLevel.ToString());
            _database.SetValue(Constants.DBREEZE_TABLE_SETTINGS, "GatewayDeviceKey", settings.GatewayDeviceKey);
            _database.SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUri", settings.AzureIotHubUri);
            _database.SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUriConnectionString", settings.AzureIotHubUriConnectionString);
            _database.SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureAccountName", settings.AzureAccountName);
            _database.SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureStorageAccessKey", settings.AzureStorageAccessKey);
            _database.SetValue(Constants.DBREEZE_TABLE_SETTINGS, "DropboxAccessToken", settings.DropboxAccessToken);

            // TODO: remove from settings
            if (string.IsNullOrEmpty(settings.Password) == false)
            {
                var user = _database.GetValue<User>(Constants.DBREEZE_TABLE_USERS, "admin");
                user.UpdatePassword(settings.Password);
                _database.SetValue(Constants.DBREEZE_TABLE_USERS, "admin", user);
                Logger.Instance.LogWarningWithContext(GetType(), "Password changed");
            }
        }

        public void Seed()
        {
            Update(new Settings
            {
                AzureAccountName = "euricomiot",
                AzureStorageAccessKey = "HXi3yoXCgM5HAjpD2q6MNFX3n0v4AtquhDero/c0l1qdi92awRwMDHwBAtIZmRrPWDvkUd1w2+j9H+jNhZvwEQ==",
                AzureIotHubUri = "EuricomIoT.azure-devices.net",
                AzureIotHubUriConnectionString = "HostName=EuricomIoT.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=1sBuf9qLaN/p5zI4XFJPrlSpk8Wfw7/K1Bd5/9yIJBA=",
                HistoryLog = 1,
                LogLevel = LogLevel.Debug,
                GatewayDeviceKey = "q5H3XA0s+XFhh+qNdfUmeJdCM9/88Hs5w59XGevZNkE=",
            });
        }
    }
}