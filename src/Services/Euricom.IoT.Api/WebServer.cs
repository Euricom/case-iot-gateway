//using Restup.Webserver.Http;
//using Restup.Webserver.Rest;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.Foundation;
////using Euricom.IoT.Api.Controllers;

//namespace Euricom.IoT.Api
//{
//    public sealed class WebServer
//    {
//        private HttpServer _httpServer;

//        public IAsyncAction InitializeWebServer()
//        {
//            var restRouteHandler = new RestRouteHandler();

//            //restRouteHandler.RegisterController<DoorLockController>();
//            //restRouteHandler.RegisterController<CameraController>();
//            //restRouteHandler.RegisterController<LazyBoneController>();

//            var configuration = new HttpServerConfiguration()
//                .ListenOnPort(8800)
//                .RegisterRoute("api", restRouteHandler)
//                .EnableCors();

//            var httpServer = new HttpServer(configuration);
//            _httpServer = httpServer;

//            // Don't release deferral, otherwise app will stop
//            return Task.Run(async () =>
//            {
//                await httpServer.StartServerAsync();
//            }).AsAsyncAction();
//        }

//    }
//}
