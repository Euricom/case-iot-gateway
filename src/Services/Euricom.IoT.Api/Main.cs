using Restup.Webserver.Attributes;
using Restup.Webserver.Http;
using Restup.Webserver.Models.Schemas;
using Restup.Webserver.Rest;
using System.Threading.Tasks;

namespace Euricom.IoT.Api
{
    internal class Main
    {
        public async Task Run()
        {
            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<ParameterController>();

            var configuration = new HttpServerConfiguration()
              .ListenOnPort(8800)
              .RegisterRoute("api", restRouteHandler)
              .EnableCors();

            var httpServer = new HttpServer(configuration);
            await httpServer.StartServerAsync();

            // now make sure the app won't stop after this (eg use a BackgroundTaskDeferral)
        }
    }

    [RestController(InstanceCreationType.Singleton)]
    internal class ParameterController
    {
        internal class DataReceived
        {
            public int ID { get; set; }
            public string PropName { get; set; }
        }

        [UriFormat("/simpleparameter/{id}/property/{propName}")]
        public GetResponse GetWithSimpleParameters(int id, string propName)
        {
            return new GetResponse(
              GetResponse.ResponseStatus.OK,
              new DataReceived() { ID = id, PropName = propName });
        }
    }
}
