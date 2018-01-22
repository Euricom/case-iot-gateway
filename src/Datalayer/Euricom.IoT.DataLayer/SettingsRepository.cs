using System;
using System.Linq;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Logging;

namespace Euricom.IoT.DataLayer
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly IotDbContext _database;

        public SettingsRepository(IotDbContext database)
        {
            _database = database;
        }

        public Settings Get()
        {
           return _database.Settings.SingleOrDefault();
        }

        public void Update(Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var s = Get();
            _database.Entry(s).CurrentValues.SetValues(settings);
            _database.SaveChanges();
        }

        private void Create(Settings settings)
        {
            _database.Settings.Add(settings);
            _database.SaveChanges();
        }

        public void Seed()
        {
            var settings = new Settings
            {
                AzureAccountName = "euricomiot",
                AzureStorageAccessKey =
                    "HXi3yoXCgM5HAjpD2q6MNFX3n0v4AtquhDero/c0l1qdi92awRwMDHwBAtIZmRrPWDvkUd1w2+j9H+jNhZvwEQ==",
                AzureIotHubUri = "EuricomIoT.azure-devices.net",
                AzureIotHubUriConnectionString =
                    "HostName=EuricomIoT.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=1sBuf9qLaN/p5zI4XFJPrlSpk8Wfw7/K1Bd5/9yIJBA=",
                HistoryLog = 1,
                LogLevel = LogLevel.Debug,
                GatewayDeviceKey = "q5H3XA0s+XFhh+qNdfUmeJdCM9/88Hs5w59XGevZNkE=",
                ZWaveNetworkKey =
                    "0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10"
            };

            if (Get() == null)
            {
                Create(settings);
            }
        }
    }
}