using AutoMapper;
using Dropbox.Api.Files;
using Euricom.IoT.Api.Manager;
using Euricom.IoT.Api.Mappings;
using Euricom.IoT.Managers;
using Euricom.IoT.ZWave;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Euricom.IoT.Api
{
    public class Startup
    {

        public async void Run()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<LazyBoneMappingProfile>();
                cfg.AddProfile<DanaLockMappingProfile>();
                cfg.AddProfile<CameraMappingProfile>();
            });

            // Init DanaLock
            await ZWaveManager.Instance.Initialize();
            Debug.WriteLine("OpenZWave initialized");

            // Init Webserver
            await new WebServer().InitializeWebServer();
            Debug.WriteLine("Restup Web Server initialized, listening on default port 8800");

            // Set up monitoring devices
            MonitorDevices();
        }

        private void MonitorDevices()
        {
            var monitoringSystem = Monitoring.MonitoringSystem.Instance; //Constructor will be called in class and then Init()
        }
    }
}
