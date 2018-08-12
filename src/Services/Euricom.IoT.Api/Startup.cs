using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Autofac;
using Euricom.IoT.Api.Mappings;
using Euricom.IoT.DataLayer;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Microsoft.EntityFrameworkCore;

namespace Euricom.IoT.Api
{
    public class Startup : IDisposable
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

            var iotDbContext = _container.Resolve<IotDbContext>();
            await iotDbContext.Database.MigrateAsync();

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
            Logger.Configure(preserveHistoryLogDays, logLevel, ApplicationData.Current.LocalFolder.Path);

            var database = _container.Resolve<IotDbContext>();
            await database.Database.MigrateAsync();

            // Init DanaLock
            var zWaveController = _container.Resolve<IZWaveController>();
            await zWaveController.Initialize(_container.Resolve<IZWaveDeviceNotificationHandler>(), settings.ZWaveNetworkKey);

            // Init storage manager
            var storageManager = _container.Resolve<IStorageManager>();
            await storageManager.Initialize();

            // Init Webserver
            await _container.Resolve<WebServer>().InitializeWebServer();

            // Start receiving and sending to IoT Hub
            var deviceGatewayRegistry = _container.Resolve<IGatewayDeviceRegistry>();
            var deviceRepository = _container.Resolve<IDeviceRepository<Device>>();

            await deviceGatewayRegistry.Initialize(deviceRepository.Get()
                .ToDictionary(d => d.DeviceId, d => d.PrimaryKey));

            // Start monitoring
            var monitors = _container.Resolve<IEnumerable<IMonitor>>();
            foreach (var monitor in monitors)
            {
                monitor.StartMonitoring();
            }
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
                cfg.AddProfile<UserMappingProfile>();
            });

            Mapper.AssertConfigurationIsValid();
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
