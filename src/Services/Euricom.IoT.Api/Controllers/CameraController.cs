using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Manager;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class CameraController
    {
        private readonly ICameraManager _cameraManager;

        public CameraController()
        {
            _cameraManager = new CameraManager();
        }

        [UriFormat("/camera")]
        public async Task<IGetResponse> GetAll()
        {
            try
            {
                var cameras = await _cameraManager.GetAll();
                return ResponseUtilities.GetResponseOk(cameras);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.GetResponseFail($"Could not get cameras: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/{deviceid}")]
        public async Task<IGetResponse> GetByDeviceId(string deviceid)
        {
            try
            {
                var cameras = await _cameraManager.Get(deviceid);
                return ResponseUtilities.GetResponseOk(cameras);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.GetResponseFail($"Could not get cameras: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/{deviceid}")]
        public async Task<IPostResponse> Add([FromContent] CameraDto camera)
        {
            try
            {
                var newCamera = await _cameraManager.Add(new Camera()
                {
                    Address = camera.Address,
                    Enabled = camera.Enabled,
                    Name = camera.Name,
                    Password = camera.Password,
                    Type = HardwareType.Camera,
                    Username = camera.Username
                });
                return ResponseUtilities.PostResponseOk(newCamera);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.PostResponseFail($"Could not add camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera")]
        public async Task<IPutResponse> Edit([FromContent] CameraDto camera)
        {
            try
            {
                var cameraEdited = await _cameraManager.Edit(new Camera()
                {
                    DeviceId = camera.DeviceId,
                    Address = camera.Address,
                    Enabled = camera.Enabled,
                    Name = camera.Name,
                    Password = camera.Password,
                    Type = HardwareType.Camera,
                    Username = camera.Username
                });
                return ResponseUtilities.PutResponseOk(cameraEdited);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.PutResponseFail($"Could not edit camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/{deviceid}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            try
            {
                var removed = await _cameraManager.Remove(deviceId);
                return ResponseUtilities.DeleteResponseOk(removed.ToString());
            }
            catch (Exception ex)
            {
                return ResponseUtilities.DeleteResponseFail($"Could not remove camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/testconnection/{deviceid}")]
        public IGetResponse TestConnection(string deviceid)
        {
            try
            {
                bool succeeded = _cameraManager.TestConnection(deviceid);
                return ResponseUtilities.GetResponseOk(succeeded);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.GetResponseFail(ex.Message);
            }
        }

        [UriFormat("/camera/notify?deviceid={deviceid}&url={url}&ts={timestamp}&frame={frameNumber}&event={eventNumber}")]
        public IGetResponse Notify(string deviceid, string url, string timestamp, int frameNumber, int eventNumber)
        {
            //Send notification to IoT hub
            _cameraManager.Notify(deviceid, url, timestamp, frameNumber, eventNumber);

            // Send response back
            return ResponseUtilities.GetResponseOk("");
        }
    }
}
