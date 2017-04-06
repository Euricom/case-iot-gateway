using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Manager;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using System.Collections.Generic;
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
                var camerasDto = Mapper.Map<IEnumerable<CameraDto>>(cameras);
                return ResponseUtilities.GetResponseOk(camerasDto);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.GetResponseFail($"Could not get cameras: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/{deviceid}")]
        public async Task<IGetResponse> GetByDeviceId(string deviceid)
        {
            try
            {
                var camera = await _cameraManager.Get(deviceid);
                var cameraDto = Mapper.Map<CameraDto>(camera);
                return ResponseUtilities.GetResponseOk(cameraDto);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.GetResponseFail($"Could not get camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera")]
        public async Task<IPostResponse> Add([FromContent] CameraDto cameraDto)
        {
            try
            {
                var camera = Mapper.Map<Camera>(cameraDto);
                var newCamera = await _cameraManager.Add(camera);
                return ResponseUtilities.PostResponseOk(newCamera);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.PostResponseFail($"Could not add camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera")]
        public async Task<IPutResponse> Edit([FromContent] CameraDto cameraDto)
        {
            try
            {
                var camera = Mapper.Map<Camera>(cameraDto);
                var cameraEdited = await _cameraManager.Edit(camera);
                return ResponseUtilities.PutResponseOk(cameraEdited);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.PutResponseFail($"Could not edit camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/{deviceid}")]
        public async Task<IDeleteResponse> Delete(string deviceid)
        {
            try
            {
                var removed = await _cameraManager.Remove(deviceid);
                return ResponseUtilities.DeleteResponseOk(removed.ToString());
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.DeleteResponseFail($"Could not remove camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/testconnection/{deviceid}")]
        public async Task<IGetResponse> TestConnection(string deviceid)
        {
            try
            {
                bool succeeded = await _cameraManager.TestConnection(deviceid);
                return ResponseUtilities.GetResponseOk(succeeded);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.GetResponseFail(ex.Message);
            }
        }

        [UriFormat("/camera/notify?deviceid={deviceid}&url={url}&ts={timestamp}&frame={frameNumber}&event={eventNumber}")]
        public IGetResponse Notify(string deviceid, string url, string timestamp, int frameNumber, int eventNumber)
        {
            try
            {
                //Send notification to IoT hub
                _cameraManager.Notify(deviceid, url, timestamp, frameNumber, eventNumber);

                // Send response back
                return ResponseUtilities.GetResponseOk("");
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.GetResponseFail(ex.Message);
            }
        }
    }
}
