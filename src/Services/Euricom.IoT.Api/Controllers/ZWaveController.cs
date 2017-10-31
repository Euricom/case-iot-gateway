using System;
using Euricom.IoT.Api.Managers;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
    [RestController(InstanceCreationType.Singleton)]
    public class ZWaveController
    {
        private readonly ZWaveManager _zWaveManager;

        public ZWaveController(ZWaveManager zWaveManager)
        {
            _zWaveManager = zWaveManager;
        }

        [UriFormat("/zwave/initialize")]
        public IPutResponse Initialize()
        {
            try
            {
                _zWaveManager.Initialize();

                return new PutResponse(PutResponse.ResponseStatus.NoContent);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not initialize ZWave: exception: {ex.Message}");
            }
        }

        [UriFormat("/zwave/nodes")]
        public IGetResponse GetNodes()
        {
            try
            {
                var nodes = _zWaveManager.GetNodes();
                return new GetResponse(GetResponse.ResponseStatus.OK, nodes);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not initialize ZWave: exception: {ex.Message}");
            }
        }
    }
}