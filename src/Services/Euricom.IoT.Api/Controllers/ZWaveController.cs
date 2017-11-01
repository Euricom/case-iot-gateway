using System;
using System.Threading.Tasks;
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

        [UriFormat("/zwave/reset/soft")]
        public async Task<IPutResponse> Initialize()
        {
            try
            {
                await _zWaveManager.SoftReset();

                return new PutResponse(PutResponse.ResponseStatus.NoContent);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not initialize ZWave: exception: {ex.Message}");
            }
        }

        [UriFormat("/zwave/node")]
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

        [UriFormat("/zwave/node/{secure}")]
        public IPostResponse AddNode(bool secure)
        {
            try
            {
                _zWaveManager.AddNode(secure);
                return new PostResponse(PostResponse.ResponseStatus.Created);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not add node: exception: {ex.Message}");
            }
        }

        [UriFormat("/zwave/node")]
        public IDeleteResponse RemoveNode()
        {
            try
            {
                _zWaveManager.RemoveNode();
                return new DeleteResponse(DeleteResponse.ResponseStatus.NoContent);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not add node: exception: {ex.Message}");
            }
        }
    }
}