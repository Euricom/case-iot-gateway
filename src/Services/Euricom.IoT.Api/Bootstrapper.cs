using System;
using System.Reflection;
using Autofac;
using Euricom.IoT.Api.Authentication;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Handlers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.AzureBlobStorage;
using Euricom.IoT.DataLayer;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.Camera;
using Euricom.IoT.Devices.DanaLock;
using Euricom.IoT.Devices.LazyBone;
using Euricom.IoT.Devices.WallMountSwitch;
using Euricom.IoT.Http;
using Euricom.IoT.Interfaces;
using Euricom.IoT.Messaging;
using Euricom.IoT.Messaging.Interfaces;
using Euricom.IoT.Models;
using Euricom.IoT.Tcp;
using Euricom.IoT.ZWave;
using Restup.Webserver.Attributes;
using Restup.Webserver.Rest;

namespace Euricom.IoT.Api
{
    public class Bootstrapper
    {
        public static IContainer Initialize()
        {
            var builder = new ContainerBuilder();

            RegisterDatabase(builder);
            RegisterRepositories(builder);
            RegisterManagers(builder);
            RegisterInfrastructure(builder);
            RegisterAzure(builder);
            RegisterControllers(builder);

            builder.RegisterType<JwtAuthenticationProvider>();
            builder.RegisterType<WebServer>().SingleInstance();

            return builder.Build();
        }

        private static void RegisterInfrastructure(ContainerBuilder builder)
        {
            builder.RegisterType<SocketClient>().As<ISocketClient>();
            builder.RegisterType<ZWaveManager>();
            builder.RegisterType<HttpService>().As<IHttpService>();
            builder.RegisterType<ZWaveController>().As<IZWaveController>().SingleInstance();
            builder.RegisterType<ZWaveMonitor>().As<IMonitor>().SingleInstance();
            builder.Register(c =>
            {
                var settings = c.Resolve<Settings>();
                return new AzureBlobStorageManager(settings.AzureAccountName, settings.AzureStorageAccessKey);
            }).As<IStorageManager>().SingleInstance();
        }

        private static void RegisterAzure(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var settings = c.Resolve<Settings>();
                return new DeviceHubRegistry(settings.AzureIotHubUriConnectionString);
            }).As<IDeviceHubRegistry>().SingleInstance();

            builder
                .RegisterType<GatewayDeviceRegistry>()
                .As<IGatewayDeviceRegistry>()
                .As<IMonitor>()
                .SingleInstance();

            builder.Register(c =>
            {
                var settings = c.Resolve<Settings>();
                return new GatewayDeviceFactory(c.Resolve<IMessageHandler>(), settings.AzureIotHubUri);
            }).As<IGatewayDeviceFactory>().SingleInstance();
        }

        private static void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<ZWaveDeviceRepository>().As<IZWaveDeviceRepository>();
            builder.RegisterType<SettingsRepository>().As<ISettingsRepository>();
            builder.RegisterType<DeviceRepository<WallMountSwitch>>().As<IDeviceRepository<WallMountSwitch>>();
            builder.RegisterType<DeviceRepository<DanaLock>>().As<IDeviceRepository<DanaLock>>();
            builder.RegisterType<DeviceRepository<Camera>>().As<IDeviceRepository<Camera>>();
            builder.RegisterType<DeviceRepository<LazyBone>>().As<IDeviceRepository<LazyBone>>();
            builder.RegisterType<DeviceRepository<Device>>().As<IDeviceRepository<Device>>();
        }

        private static void RegisterManagers(ContainerBuilder builder)
        {
            builder.RegisterType<CameraManager>().As<ICameraManager>();
            builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>();
            builder.RegisterType<DanaLockManager>().As<IDanaLockManager>();
            builder.RegisterType<LazyBoneManager>().As<ILazyBoneManager>();
            builder.RegisterType<LogManager>().As<ILogManager>();
            builder.RegisterType<SecurityManager>().As<ISecurityManager>();
            builder.RegisterType<WallMountSwitchManager>().As<IWallMountSwitchManager>();
            builder.RegisterType<ZWaveManager>().As<IZWaveManager>();
            builder.RegisterType<UserManager>().As<IUserManager>();

            builder.RegisterType<MessageHandler>().As<IMessageHandler>();
            builder.RegisterType<ZWaveDeviceNotificationHandler>().As<IZWaveDeviceNotificationHandler>().SingleInstance();
        }

        private static void RegisterDatabase(ContainerBuilder builder)
        {
            builder.RegisterType<IotDbContext>();
            builder.Register(context =>
            {
                var repository = context.Resolve<ISettingsRepository>();
                return repository.Get();
            });
        }

        private static void RegisterControllers(ContainerBuilder builder)
        {
            // Register controllers
            foreach (Type type in typeof(Bootstrapper).GetTypeInfo().Assembly.GetTypes())
            {
                var attribute = type.GetTypeInfo().GetCustomAttribute<RestControllerAttribute>();
                if (attribute != null)
                {
                    builder.RegisterType(type);
                }
            }

            builder.RegisterType<GlobalExceptionHandler>().As<IExceptionHandler>();
        }
    }
}