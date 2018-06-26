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
    [Authorize("Manager")]
    [RestController]
    public class CameraController
    {
        private readonly ICameraManager _cameraManager;

        public CameraController(ICameraManager cameraManager)
        {
            _cameraManager = cameraManager;
        }

        [UriFormat("/camera")]
        public IGetResponse GetAll()
        {
            var cameras = _cameraManager.Get();
            return ResponseUtilities.GetResponseOk(cameras);
        }

        [UriFormat("/camera/{deviceId}")]
        public IGetResponse Get(string deviceId)
        {
            var camera = _cameraManager.Get(deviceId);
            return ResponseUtilities.GetResponseOk(camera);
        }

        [UriFormat("/camera")]
        public async Task<IPostResponse> Add([FromContent] CameraDto dto)
        {
            var newCamera = await _cameraManager.Add(dto);
            return ResponseUtilities.PostResponseOk(newCamera);
        }

        [UriFormat("/camera")]
        public IPutResponse Edit([FromContent] CameraDto dto)
        {
            var camera = _cameraManager.Update(dto);
            return ResponseUtilities.PutResponseOk(camera);
        }

        [UriFormat("/camera/{deviceId}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            await _cameraManager.Remove(deviceId);
            return new DeleteResponse(DeleteResponse.ResponseStatus.NoContent);
        }

        [UriFormat("/camera/testconnection/{deviceId}")]
        public async Task<IGetResponse> TestConnection(string deviceId)
        {
            bool succeeded = await _cameraManager.TestConnection(deviceId);
            return ResponseUtilities.GetResponseOk(succeeded);
        }

        [UriFormat("/camera/notify?devicename={devicename}&url={url}&ts={timestamp}&frame={frameNumber}&event={eventNumber}")]
        public IGetResponse Notify(string devicename, string url, string timestamp, int frameNumber, int eventNumber)
        {
            try
            {
                //Send notification to IoT hub
                //var deviceId = new HardwareManager().GetDeviceId(devicename);
                //_cameraManager.Notify(deviceId, url, timestamp, frameNumber, eventNumber);

                // Send response back
                return ResponseUtilities.GetResponseOk("");
            }
            catch (Exception ex)
            {                
                //Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
