using System.Threading.Tasks;
using Euricom.IoT.Api.Managers.Interfaces;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize("Administrator")]
    [RestController]
    public class ZWaveController
    {
        private readonly IZWaveManager _zWaveManager;

        public ZWaveController(IZWaveManager zWaveManager)
        {
            _zWaveManager = zWaveManager;
        }

        [UriFormat("/zwave/reset/soft")]
        public async Task<IPutResponse> Initialize()
        {
            await _zWaveManager.SoftReset();
            return new PutResponse(PutResponse.ResponseStatus.NoContent);
        }

        [UriFormat("/zwave/node")]
        public IGetResponse GetNodes()
        {
            var nodes = _zWaveManager.GetNodes();
            return new GetResponse(GetResponse.ResponseStatus.OK, nodes);
        }

        [UriFormat("/zwave/status")]
        public IGetResponse GetStatus()
        {
            var status = _zWaveManager.GetStatus();
            return new GetResponse(GetResponse.ResponseStatus.OK, status);
        }

        [UriFormat("/zwave/node/{secure}")]
        public IPostResponse AddNode(bool secure)
        {
            _zWaveManager.AddNode(secure);
            return new PostResponse(PostResponse.ResponseStatus.Created);
        }

        [UriFormat("/zwave/node")]
        public IDeleteResponse RemoveNode()
        {
            _zWaveManager.RemoveNode();
            return new DeleteResponse(DeleteResponse.ResponseStatus.NoContent);
        }
    }
}