using AutoMapper;
using Euricom.IoT.Api.Mappings;
using Euricom.IoT.Logging;
using System;
using System.Threading.Tasks;
using Autofac;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Models;
using Euricom.IoT.ZWave.Interfaces;

namespace Euricom.IoT.Api
{
    public class Startup: IDisposable
    {
        private readonly IContainer _container;

        public Startup()
        {
            _container = Bootstrapper.Initialize();
        }

        public async void Run()
        {
            // Add AutoMapper mappings
            AddAutoMapperMappings();

            // Init admin user
            var userRepository = _container.Resolve<IUserRepository>();
            userRepository.Seed();

            var settingsRepository = _container.Resolve<ISettingsRepository>();
            settingsRepository.Seed();

            var settings = _container.Resolve<Settings>();
            
            // Get setting for preserving history log (days)
            var preserveHistoryLogDays = settings.HistoryLog;
            // Get setting for log level
            var logLevel = settings.LogLevel;

            // Init logger
            Logger.Configure(preserveHistoryLogDays, logLevel);
            var instLogger = Logger.Instance;

            // Init DanaLock
            var zWaveManager = _container.Resolve<IZWaveManager>();
            await zWaveManager.Initialize();

            // Set up monitoring of devices / regular tasks that cleanup files
            //StartMonitors();

            // Init Webserver
            await _container.Resolve<WebServer>().InitializeWebServer();

            // Process incoming IoT Hub messages
            //await new GatewayManager().Initialize();
        }

        private static void AddAutoMapperMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<SettingsMappingProfile>();
                cfg.AddProfile<LazyBoneMappingProfile>();
                cfg.AddProfile<DanaLockMappingProfile>();
                cfg.AddProfile<WallMountMappingProfile>();
                cfg.AddProfile<NodeMappingProfile>();
                cfg.AddProfile<CameraMappingProfile>();
                cfg.AddProfile<LogMappingProfile>();
            });
        }

        private void StartMonitors()
        {
            //var monitoringSystem = Monitoring.MonitoringSystem.Instance; //Constructor will be called in class and then Init()
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
