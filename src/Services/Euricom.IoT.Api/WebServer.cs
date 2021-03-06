﻿using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Euricom.IoT.Api.Controllers;
using Restup.Webserver.File;
using System.Diagnostics;
using Restup.WebServer.Http;
using Euricom.IoT.Api.Authentication;

namespace Euricom.IoT.Api
{
    public class WebServer
    {
        private HttpServer _httpServer;

        public async Task InitializeWebServer()
        {
            var authProvider = new JwtAuthenticationProvider(null, null);
            var restRouteHandler = new RestRouteHandler(authProvider);
            restRouteHandler.RegisterController<ConfigurationController>();
            restRouteHandler.RegisterController<DevicesController>();
            restRouteHandler.RegisterController<CameraController>();
            restRouteHandler.RegisterController<DanaLockController>();
            restRouteHandler.RegisterController<LazyBoneController>();
            restRouteHandler.RegisterController<WallMountSwitchController>();
            restRouteHandler.RegisterController<LogController>();
            restRouteHandler.RegisterController<SecurityController>();

            var configuration = new HttpServerConfiguration()
              .ListenOnPort(8800)
              .RegisterRoute("api", restRouteHandler)
              .RegisterRoute(new StaticFileRouteHandler(@"Euricom.IoT.UI.WebAdministration\Web"))
              .EnableCors();

            _httpServer = new HttpServer(configuration);
            await _httpServer.StartServerAsync();

            Debug.WriteLine("Restup Web Server initialized, listening on default port 8800");
        }
    }
}
