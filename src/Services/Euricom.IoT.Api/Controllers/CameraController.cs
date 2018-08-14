using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.WebServer.Attributes;
using System;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;
using Restup.Webserver.Models.Schemas;

namespace Euricom.IoT.Api.Controllers
{
    [RestController]
    public class CameraController
    {
        private readonly ICameraManager _cameraManager;

        public CameraController(ICameraManager cameraManager)
        {
            _cameraManager = cameraManager;
        }

        [Authorize("Manager")]
        [UriFormat("/camera")]
        public IGetResponse GetAll()
        {
            var cameras = _cameraManager.Get();
            return ResponseUtilities.GetResponseOk(cameras);
        }

        [Authorize("Manager")]
        [UriFormat("/camera/{deviceId}")]
        public IGetResponse Get(string deviceId)
        {
            var camera = _cameraManager.Get(deviceId);
            return ResponseUtilities.GetResponseOk(camera);
        }

        [Authorize("Manager")]
        [UriFormat("/camera")]
        public async Task<IPostResponse> Add([FromContent] CameraDto dto)
        {
            var newCamera = await _cameraManager.Add(dto);
            return ResponseUtilities.PostResponseOk(newCamera);
        }

        [Authorize("Manager")]
        [UriFormat("/camera")]
        public IPutResponse Edit([FromContent] CameraDto dto)
        {
            var camera = _cameraManager.Update(dto);
            return ResponseUtilities.PutResponseOk(camera);
        }

        [Authorize("Manager")]
        [UriFormat("/camera/{deviceId}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            await _cameraManager.Remove(deviceId);
            return new DeleteResponse(DeleteResponse.ResponseStatus.NoContent);
        }

        [Authorize("Manager")]
        [UriFormat("/camera/testconnection/{deviceId}")]
        public async Task<IGetResponse> TestConnection(string deviceId)
        {
            bool succeeded = await _cameraManager.TestConnection(deviceId);
            return ResponseUtilities.GetResponseOk(succeeded);
        }

        [Authorize("Manager")]
        [UriFormat("/camera/{deviceId}/picture")]
        public async Task<IGetResponse> GetPicture(string deviceId)
        {
            var url = await _cameraManager.GetPicture(deviceId, null);
            return ResponseUtilities.GetResponseOk(url);
        }

        [UriFormat("/camera/{deviceId}/notify?fileName={fileName}&timestamp={timestamp}")]
        public IGetResponse Notify(string deviceId, string fileName, DateTime timestamp)
        {
            try
            {
#pragma warning disable 4014
                // Just fire off and forget.
                Task.Run(() =>
                {
                    try
                    {
                        _cameraManager.Notify(deviceId, fileName, timestamp);
                    }
                    catch (Exception ex)
                    {
                        Logging.Logger.Instance.Error(ex, $"Could not get image: {ex.Message}");
                    }
                });
#pragma warning restore 4014

                return new GetResponse(GetResponse.ResponseStatus.OK);
            }
            catch (Exception ex)
            {                
                Logging.Logger.Instance.Error(ex);
                return new GetResponse(GetResponse.ResponseStatus.OK);
            }
        }
    }
}
