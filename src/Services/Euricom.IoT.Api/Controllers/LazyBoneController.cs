using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Devices.LazyBone;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize("Manager")]
    [RestController]
    public class LazyBoneController
    {
        private readonly ILazyBoneManager _lazyBoneManager;

        public LazyBoneController(ILazyBoneManager lazyBoneManager)
        {
            _lazyBoneManager = lazyBoneManager;
        }

        [UriFormat("/lazybone")]
        public IGetResponse GetAll()
        {
            var lazyBones = _lazyBoneManager.Get();
            return ResponseUtilities.GetResponseOk(lazyBones);
        }

        [UriFormat("/lazybone")]
        public async Task<PostResponse> Add([FromContent] LazyBoneDto lazyBoneDto)
        {
            var newLazyBone = await _lazyBoneManager.Add(lazyBoneDto);
            return ResponseUtilities.PostResponseOk(newLazyBone);
        }

        [UriFormat("/lazybone")]
        public IPutResponse Edit([FromContent] LazyBoneDto lazyBoneDto)
        {
            var lazyBoneEdited = _lazyBoneManager.Update(lazyBoneDto);
            return ResponseUtilities.PutResponseOk(lazyBoneEdited);
        }

        [UriFormat("/lazybone/{deviceId}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            await _lazyBoneManager.Remove(deviceId);
            return ResponseUtilities.DeleteResponseOk();
        }

        [UriFormat("/lazybone/{deviceId}/testconnection")]
        public IGetResponse TestConnection(string deviceId)
        {
            var result = _lazyBoneManager.TestConnection(deviceId);
            return ResponseUtilities.GetResponseOk(result);
        }

        [UriFormat("/lazybone/{deviceId}/state")]
        public IGetResponse GetState(string deviceId)
        {
            var state = _lazyBoneManager.GetState(deviceId);
            return ResponseUtilities.GetResponseOk(state);
        }

        [UriFormat("/lazybone/{deviceId}/state")]
        public IPutResponse SetState(string deviceId, [FromContent] LazyBoneState state)
        {
            _lazyBoneManager.SetState(deviceId, state);
            return new PutResponse(PutResponse.ResponseStatus.NoContent);
        }
    }
}
