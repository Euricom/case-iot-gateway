using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;
using System;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Devices.LazyBone;
using Euricom.IoT.Logging;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
    [RestController(InstanceCreationType.Singleton)]
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
            try
            {
                var lazyBones = _lazyBoneManager.Get();
                return ResponseUtilities.GetResponseOk(lazyBones);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not get lazyBones: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone")]
        public async Task<PostResponse> Add([FromContent] LazyBoneDto lazyBoneDto)
        {
            try
            {
                var newLazyBone = await _lazyBoneManager.Add(lazyBoneDto);
                return ResponseUtilities.PostResponseOk(newLazyBone);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not add lazyBone: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone")]
        public IPutResponse Edit([FromContent] LazyBoneDto lazyBoneDto)
        {
            try
            {
                var lazyBoneEdited = _lazyBoneManager.Update(lazyBoneDto);
                return ResponseUtilities.PutResponseOk(lazyBoneEdited);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not edit lazyBone: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/{deviceId}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            try
            {
                await _lazyBoneManager.Remove(deviceId);
                return ResponseUtilities.DeleteResponseOk();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not remove lazyBone: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/{deviceId}/testconnection")]
        public IGetResponse TestConnection(string deviceId)
        {
            try
            {
                var result = _lazyBoneManager.TestConnection(deviceId);
                return ResponseUtilities.GetResponseOk(result);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw new Exception(ex.Message);
            }
        }

        [UriFormat("/lazybone/{deviceId}/state")]
        public IGetResponse GetState(string deviceId)
        {
            try
            {
                var state = _lazyBoneManager.GetState(deviceId);
                return ResponseUtilities.GetResponseOk(state);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw new Exception($"Could not determine lazybone state: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/{deviceId}/state")]
        public IPutResponse SetState(string deviceId, [FromContent] LazyBoneState state)
        {
            try
            {
                _lazyBoneManager.SetState(deviceId, state);
                return new PutResponse(PutResponse.ResponseStatus.NoContent);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw new Exception($"Could not determine lazybone state: exception: {ex.Message}");
            }
        }
    }
}
