using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Euricom.IoT.Api.Controllers;

namespace Euricom.IoT.Api
{
    public class WebServer
    {
        private HttpServer _httpServer;

        public async Task InitializeWebServer()
        {
            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<ConfigurationController>();
            restRouteHandler.RegisterController<CameraController>();
            restRouteHandler.RegisterController<DanaLockController>();
            restRouteHandler.RegisterController<LazyBoneController>();

            var configuration = new HttpServerConfiguration()
              .ListenOnPort(8800)
              .RegisterRoute("api", restRouteHandler)
              .EnableCors();

            _httpServer = new HttpServer(configuration);
            await _httpServer.StartServerAsync();
        }
    }
}
