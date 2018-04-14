using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;
using System;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
    [RestController(InstanceCreationType.Singleton)]
    public class AccessTokenController
    {

    }

    [Authorize]
    [RestController(InstanceCreationType.Singleton)]
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
            try
            {
                var cameras = _cameraManager.Get();
                return ResponseUtilities.GetResponseOk(cameras);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not get cameras: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/{deviceId}")]
        public IGetResponse Get(string deviceId)
        {
            try
            {
                var camera = _cameraManager.Get(deviceId);
                return ResponseUtilities.GetResponseOk(camera);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not get camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera")]
        public async Task<IPostResponse> Add([FromContent] CameraDto dto)
        {
            try
            {
                if (Common.Validation.ValidateDeviceId(dto.DeviceId))
                {
                    throw new ArgumentException(nameof(dto.DeviceId));
                }

                var newCamera = await _cameraManager.Add(dto);
                return ResponseUtilities.PostResponseOk(newCamera);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not add camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera")]
        public IPutResponse Edit([FromContent] CameraDto dto)
        {
            try
            {
                var camera = _cameraManager.Update(dto);
                return ResponseUtilities.PutResponseOk(camera);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not edit camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/{deviceId}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            try
            {
                await _cameraManager.Remove(deviceId);
                return ResponseUtilities.DeleteResponseOk();
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not remove camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/testconnection/{deviceId}")]
        public async Task<IGetResponse> TestConnection(string deviceId)
        {
            try
            {
                bool succeeded = await _cameraManager.TestConnection(deviceId);
                return ResponseUtilities.GetResponseOk(succeeded);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception(ex.Message);
            }
        }

        //[UriFormat("/camera/notify?devicename={devicename}&url={url}&ts={timestamp}&frame={frameNumber}&event={eventNumber}")]
        //public IGetResponse Notify(string devicename, string url, string timestamp, int frameNumber, int eventNumber)
        //{
        //    try
        //    {
        //        //Send notification to IoT hub
        //        var deviceId = _cameraManager.GetDeviceId(devicename);
        //        _cameraManager.Notify(deviceId, url, timestamp, frameNumber, eventNumber);

        //        // Send response back
        //        return ResponseUtilities.GetResponseOk("");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
        //        throw new Exception(ex.Message);
        //    }
        //}
    }
}
