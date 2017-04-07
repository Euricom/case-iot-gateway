using AutoMapper;
using Euricom.IoT.Api.Mappings;
using Euricom.IoT.Logging;
using Euricom.IoT.ZWave;
using Serilog;
using System;
using System.Diagnostics;
using Windows.Storage;

namespace Euricom.IoT.Api
{
    public class Startup
    {
        public async void Run()
        {
            // Add AutoMapper mappings
            AddAutoMapperMappings();

            // Init Database
            var instDatabase = DataLayer.Database.Instance;

            // Get setting for preserving history log (days)
            var preserveHistoryLogDays = instDatabase.GetConfigSettings().HistoryLog;

            // Init logger
            Logger.Configure(preserveHistoryLogDays);
            var instLogger = Logger.Instance;

            // Init DanaLock
            await ZWaveManager.Instance.Initialize();

            // Init Webserver
            await new WebServer().InitializeWebServer();

            // Set up monitoring devices
            // MonitorDevices();
        }

        private static void AddAutoMapperMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<LazyBoneMappingProfile>();
                cfg.AddProfile<DanaLockMappingProfile>();
                cfg.AddProfile<CameraMappingProfile>();
                cfg.AddProfile<LogMappingProfile>();
            });
        }

        private void MonitorDevices()
        {
            var monitoringSystem = Monitoring.MonitoringSystem.Instance; //Constructor will be called in class and then Init()
        }
    }
}
