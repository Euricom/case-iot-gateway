using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize("Manager")]
    [RestController]
    public class WallMountSwitchController : ControllerBase
    {
        private readonly IWallMountSwitchManager _wallmountSwitchManager;

        public WallMountSwitchController(IWallMountSwitchManager wallMountSwitchManager)
        {
            _wallmountSwitchManager = wallMountSwitchManager;
        }

        [UriFormat("/wallmount")]
        public IGetResponse GetAll()
        {
            var wallmounts = _wallmountSwitchManager.Get();
            return ResponseUtilities.GetResponseOk(wallmounts);
        }

        [UriFormat("/wallmount/{deviceId}")]
        public IGetResponse GetById(string deviceId)
        {
            var wallmount = _wallmountSwitchManager.Get(deviceId);
            return ResponseUtilities.GetResponseOk(wallmount);
        }

        [UriFormat("/wallmount")]
        public async Task<IPostResponse> Add([FromContent] WallMountSwitchDto dto)
        {
            var wallmount = await _wallmountSwitchManager.Add(dto);
            return ResponseUtilities.PostResponseOk(wallmount);
        }

        [UriFormat("/wallmount")]
        public IPutResponse Update([FromContent] WallMountSwitchDto dto)
        {
            var wallmount = _wallmountSwitchManager.Update(dto);
            return ResponseUtilities.PutResponseOk(wallmount);
        }

        [UriFormat("/wallmount/{deviceId}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            await _wallmountSwitchManager.Remove(deviceId);

            return new DeleteResponse(DeleteResponse.ResponseStatus.NoContent);
        }

        [UriFormat("/wallmount/{deviceId}/testconnection")]
        public IGetResponse TestConnection(string deviceId)
        {
            bool succeeded = _wallmountSwitchManager.TestConnection(deviceId);

            return ResponseUtilities.GetResponseOk(succeeded);
        }

        [UriFormat("/wallmount/{deviceId}/state")]
        public IGetResponse GetState(string deviceId)
        {
            var isOn = _wallmountSwitchManager.IsOn(deviceId);
            return ResponseUtilities.GetResponseOk(isOn.ToString());
        }

        [UriFormat("/wallmount/{deviceId}/switch/{state}")]
        public IPutResponse Switch(string deviceId, string state)
        {
            //Send switch command to the manager
            _wallmountSwitchManager.Switch(deviceId, state);

            //If it works, send response back to client
            return new PutResponse(PutResponse.ResponseStatus.NoContent);
        }
    }
}
