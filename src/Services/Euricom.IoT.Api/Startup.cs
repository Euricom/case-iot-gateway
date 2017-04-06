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

            var instLogger = Logger.Instance;

            // Init DanaLock
            // await ZWaveManager.Instance.Initialize();
            Debug.WriteLine("OpenZWave initialized");

            // Init Webserver
            await new WebServer().InitializeWebServer();
            Debug.WriteLine("Restup Web Server initialized, listening on default port 8800");

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
