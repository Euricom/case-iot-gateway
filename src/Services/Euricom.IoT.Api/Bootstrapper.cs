using System;
using Windows.Storage;
using Autofac;
using DBreeze;
using Euricom.IoT.Api.Manager;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureBlobStorage;
using Euricom.IoT.DataLayer;
using Euricom.IoT.Logging;
using Euricom.IoT.Mailing;
using Euricom.IoT.ZWave;
using Restup.WebServer.Http;

namespace Euricom.IoT.Api
{
    public class Bootstrapper
    {
        public static IContainer Initialize()
        {
            var builder = new ContainerBuilder();

            RegisterDatabase(builder);
            builder.RegisterType<ZWaveManager>().As<IZWaveManager>().SingleInstance();
            builder.RegisterType<CameraManager>().As<ICameraManager>();
            builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>();
            builder.RegisterType<DanaLockManager>().As<IDanaLockManager>();
            builder.RegisterType<GatewayManager>().As<IGatewayManager>();
            builder.RegisterType<LazyBoneManager>().As<ILazyBoneManager>();
            builder.RegisterType<LogManager>().As<ILogManager>();
            builder.RegisterType<SecurityManager>().As<ISecurityManager>();
            builder.RegisterType<WallMountSwitchManager>().As<IWallMountSwitchManager>();

            builder.RegisterType<WallMountSwitch.WallMountSwitchManager>().As<WallMountSwitch.IWallMountSwitchManager>();
            builder.RegisterType<DanaLock.DanaLockManager>().As<DanaLock.IDanaLockManager>();
            builder.RegisterType<LazyBone.LazyBoneConnectionManager>();
            builder.Register(context => ZWaveManager.Instance).As<IZWaveManager>().SingleInstance();

            builder.RegisterType<AzureBlobStorageManager>().As<IAzureBlobStorageManager>();
            builder.RegisterType<Mailer>();
            builder.Register(context => new JwtAuthenticationProvider(null, null, context.Resolve<ISecurityManager>()));
            builder.RegisterType<WebServer>().SingleInstance();

            return builder.Build();
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
            builder.RegisterType<Database>().AsSelf().As<IDatabase>().SingleInstance();
            builder.Register(context =>
            {
                var database = context.Resolve<Database>();
                return database.GetConfigSettings();
            });
        }
    }
}