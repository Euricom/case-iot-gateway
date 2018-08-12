using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.WebServer.Attributes;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;
using Restup.Webserver.Models.Schemas;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize("Manager")]
    [RestController]
    public class DanaLockController
    {
        private readonly IDanaLockManager _danaLockManager;

        public DanaLockController(IDanaLockManager danaLockManager)
        {
            _danaLockManager = danaLockManager;
        }

        [UriFormat("/danalock")]
        public IGetResponse GetAll()
        {
            var locks = _danaLockManager.Get();
            return ResponseUtilities.GetResponseOk(locks);
        }

        [UriFormat("/danalock")]
        public async Task<IPostResponse> Add([FromContent] DanaLockDto dto)
        {
            var danalock = await _danaLockManager.Add(dto);
            return ResponseUtilities.PostResponseOk(danalock);
        }

        [UriFormat("/danalock")]
        public IPutResponse Edit([FromContent] DanaLockDto dto)
        {
            var danalock = _danaLockManager.Update(dto);
            return ResponseUtilities.PutResponseOk(danalock);
        }

        [UriFormat("/danalock/{deviceId}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            await _danaLockManager.Remove(deviceId);
            return new DeleteResponse(DeleteResponse.ResponseStatus.NoContent);
        }

        [UriFormat("/danalock/{deviceId}/testconnection")]
        public IGetResponse TestConnection(string deviceId)
        {
            bool succeeded = _danaLockManager.TestConnection(deviceId);
            return ResponseUtilities.GetResponseOk(succeeded);
        }

        [UriFormat("/danalock/{deviceId}/islocked")]
        public IGetResponse IsLocked(string deviceId)
        {
            var isLocked = _danaLockManager.IsLocked(deviceId);
            return ResponseUtilities.GetResponseOk(isLocked.ToString());
        }

        [UriFormat("/danalock/{deviceId}/switch/{state}")]
        public IPutResponse Switch(string deviceId, string state)
        {
            //UpdateStateAsync switch command to the manager
            _danaLockManager.Switch(deviceId, state);

            //If it works, send response back to client
            return new PutResponse(PutResponse.ResponseStatus.NoContent);
        }
    }
}
