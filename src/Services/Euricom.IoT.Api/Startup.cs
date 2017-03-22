
using Euricom.IoT.Api.Controllers;
using Euricom.IoT.DataLayer;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using static Euricom.IoT.DataLayer.ConfigReader;

namespace Euricom.IoT.Api
{
    public class Startup
    {
        private HttpServer _httpServer;
        private BackgroundTaskDeferral _deferral;
        private DeviceConfigurations _deviceConfigs;

        public async void Run()
        {
            // Read configuration
            _deviceConfigs = new ConfigReader().ReadConfiguration();

            // Init DanaLock
            //await InitDanaLock();

            // Init Webserver
            await InitWebServer();
        }

        //private async Task InitDanaLock()
        //{
        //    //await DanaLock.Instance.Initialize();
        //}

        private async Task InitWebServer()
        {
            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<CameraController>(_deviceConfigs.CameraConfig);
            restRouteHandler.RegisterController<DoorLockController>(_deviceConfigs.DoorConfig);
            restRouteHandler.RegisterController<LazyBoneController>(_deviceConfigs.LazyBoneConfig);

            var configuration = new HttpServerConfiguration()
              .ListenOnPort(8800)
              .RegisterRoute("api", restRouteHandler)
              .EnableCors();

            _httpServer = new HttpServer(configuration);
            await _httpServer.StartServerAsync();
        }
    }
}
