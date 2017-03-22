using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System.Threading.Tasks;

namespace ClassLibraryApi
{
    public sealed class WebServer
    {
        private HttpServer _httpServer;

        public async Task InitializeWebServer()
        {
            var restRouteHandler = new RestRouteHandler();

            //restRouteHandler.RegisterController<DoorLockController>();
            //restRouteHandler.RegisterController<CameraController>();
            //restRouteHandler.RegisterController<LazyBoneController>();

            var configuration = new HttpServerConfiguration()
                .ListenOnPort(8800)
                .RegisterRoute("api", restRouteHandler)
                .EnableCors();

            var httpServer = new HttpServer(configuration);
            _httpServer = httpServer;

            // Don't release deferral, otherwise app will stop
            await httpServer.StartServerAsync();
        }
    }
}
