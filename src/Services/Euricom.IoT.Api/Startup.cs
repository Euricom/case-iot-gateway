using AutoMapper;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Mappings;
using Euricom.IoT.Logging;
using Euricom.IoT.Models.Messages;
using Euricom.IoT.ZWave;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api
{
    public class Startup
    {
        public async void Run()
        {
            // Add AutoMapper mappings
            AddAutoMapperMappings();

            // Init Database
            var db = DataLayer.Database.Instance;
            var settings = db.GetConfigSettings();

            // Get setting for preserving history log (days)
            var preserveHistoryLogDays = settings.HistoryLog;
            // Get setting for log level
            var logLevel = settings.LogLevel;

            // Init logger
            Logger.Configure(preserveHistoryLogDays, logLevel);
            var instLogger = Logger.Instance;

            // Init DanaLock
            await ZWaveManager.Instance.Initialize();

            // Init Webserver
            await new WebServer().InitializeWebServer();

            // Process incoming IoT Hub messages
            // await new GatewayManager().Initialize();

            // Set up monitoring devices
            // MonitorDevices();
        }

        private static void AddAutoMapperMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<SettingsMappingProfile>();
                cfg.AddProfile<LazyBoneMappingProfile>();
                cfg.AddProfile<DanaLockMappingProfile>();
                cfg.AddProfile<WallMountMappingProfile>();
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
