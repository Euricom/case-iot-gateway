using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Manager;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Models;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
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

        [UriFormat("/camera/{devicename}")]
        public async Task<IGetResponse> GetByDeviceName(string devicename)
        {
            try
            {
                var camera = await _cameraManager.GetByDeviceName(devicename);
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

        [UriFormat("/camera/{devicename}")]
        public async Task<IDeleteResponse> Delete(string devicename)
        {
            try
            {
                var removed = await _cameraManager.Remove(devicename);
                return ResponseUtilities.DeleteResponseOk(removed.ToString());
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.DeleteResponseFail($"Could not remove camera: exception: {ex.Message}");
            }
        }

        [UriFormat("/camera/testconnection/{devicename}")]
        public async Task<IGetResponse> TestConnection(string devicename)
        {
            try
            {
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                bool succeeded = await _cameraManager.TestConnection(deviceId);
                return ResponseUtilities.GetResponseOk(succeeded);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.GetResponseFail(ex.Message);
            }
        }

        [UriFormat("/camera/notify?devicename={devicename}&url={url}&ts={timestamp}&frame={frameNumber}&event={eventNumber}")]
        public IGetResponse Notify(string devicename, string url, string timestamp, int frameNumber, int eventNumber)
        {
            try
            {
                //Send notification to IoT hub
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                _cameraManager.Notify(deviceId, url, timestamp, frameNumber, eventNumber);

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
