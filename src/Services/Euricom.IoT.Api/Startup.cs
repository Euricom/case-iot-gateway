using AutoMapper;
using Euricom.IoT.Api.Mappings;
using Euricom.IoT.Common.Logging;
using Euricom.IoT.Logging;
using Euricom.IoT.ZWave;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Serilog;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
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

            // Wait for messages
            // ForwardMessagesToDevices();

            // Set up monitoring devices
            // MonitorDevices();
        }

        private void ForwardMessagesToDevices()
        {
            Logger.Instance.LogInformationWithContext(this.GetType(), "Forwarding IoT hub messages from IoTGateway device to hardware");
            var settings = DataLayer.Database.Instance.GetConfigSettings();
            if (settings == null || String.IsNullOrEmpty(settings.GatewayDeviceKey))
            {
                Logger.Instance.LogWarningWithContext(this.GetType(), "Setting: 'Gateway Device key' was not provided.. Cannot proccess device messages");
                return;
            }

            var deviceGatewayClient = DeviceClient.Create(settings.AzureIotHubUri, 
                new DeviceAuthenticationWithRegistrySymmetricKey("IoTGateway", settings.GatewayDeviceKey), 
                Microsoft.Azure.Devices.Client.TransportType.Http1);

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        Microsoft.Azure.Devices.Client.Message receivedMessage = await deviceGatewayClient.ReceiveAsync();
                        if (receivedMessage == null) continue;

                        var messageString = Encoding.ASCII.GetString(receivedMessage.GetBytes());

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Received message: {0}", messageString);
                        Console.ResetColor();

                        await deviceGatewayClient.CompleteAsync(receivedMessage);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                    }
                }
            });
        }

        private static void AddAutoMapperMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<SettingsMappingProfile>();
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
