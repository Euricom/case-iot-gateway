﻿using AutoMapper;
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
using Autofac;
using Euricom.IoT.Models;

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
