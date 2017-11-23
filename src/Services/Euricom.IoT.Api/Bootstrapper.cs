﻿using System;
using Windows.Storage;
using Autofac;
using DBreeze;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.DataLayer;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.Camera;
using Euricom.IoT.Devices.DanaLock;
using Euricom.IoT.Devices.WallMountSwitch;
using Euricom.IoT.Http;
using Euricom.IoT.Http.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Mailing;
using Euricom.IoT.Models;
using Restup.WebServer.Http;

namespace Euricom.IoT.Api
{
    public class Bootstrapper
    {
        public static IContainer Initialize()
        {
            var builder = new ContainerBuilder();

            RegisterDatabase(builder);
            RegisterRepositories(builder);


            builder.RegisterType<ZWaveManager>();
            builder.RegisterType<ZWaveManager>().As<IZWaveManager>().SingleInstance();
            builder.RegisterType<CameraManager>().As<ICameraManager>();
            builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>();
            builder.RegisterType<DanaLockManager>().As<IDanaLockManager>();
            builder.RegisterType<GatewayManager>().As<IGatewayManager>();
            builder.RegisterType<LazyBoneManager>().As<ILazyBoneManager>();
            builder.RegisterType<LogManager>().As<ILogManager>();
            builder.RegisterType<SecurityManager>().As<ISecurityManager>();
            builder.RegisterType<WallMountSwitchManager>().As<IWallMountSwitchManager>();
            builder.RegisterType<HttpService>().As<IHttpService>();
            
            builder.Register(context =>
            {
                var settings = context.Resolve<Settings>();

                return new ZWave.ZWaveManager(settings.ZWaveNetworkKey);
            }).As<ZWave.Interfaces.IZWaveManager>().SingleInstance();

            //builder.RegisterType<AzureBlobStorageManager>().As<IAzureBlobStorageManager>();
            builder.RegisterType<Mailer>();
            builder.Register(context => new JwtAuthenticationProvider(null, null, context.Resolve<ISecurityManager>()));
            builder.RegisterType<WebServer>().SingleInstance();

            return builder.Build();
        }

        private static void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<SettingsRepository>().As<ISettingsRepository>();
            builder.RegisterType<DeviceRepository<WallMountSwitch>>().As<IDeviceRepository<WallMountSwitch>>();
            builder.RegisterType<DeviceRepository<DanaLock>>().As<IDeviceRepository<DanaLock>>();
            builder.RegisterType<DeviceRepository<Camera>>().As<IDeviceRepository<Camera>>();
        }

        private static void RegisterDatabase(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                try
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    var engine = new DBreezeEngine(new DBreezeConfiguration { DBreezeDataFolderName = localFolder.Path });
                    Logger.Instance.LogInformationWithContext(typeof(Bootstrapper), "Database DBreeze (settings DB) Initialized succesfully");

                    return engine;
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogErrorWithContext(typeof(Bootstrapper), ex);
                    throw;
                }
            }).SingleInstance();
            builder.RegisterType<DbBreezeDatabase>().As<IDbBreezeDatabase>();
            builder.Register(context =>
            {
                var repository = context.Resolve<ISettingsRepository>();
                return repository.Get();
            });
        }
    }
}