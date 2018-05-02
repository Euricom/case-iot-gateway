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
    [Authorize("Manager")]
    [RestController]
    public class WallMountSwitchController: ControllerBase
    {
        private readonly IWallMountSwitchManager _wallmountSwitchManager;

        public WallMountSwitchController(IWallMountSwitchManager wallMountSwitchManager)
        {
            _wallmountSwitchManager = wallMountSwitchManager;
        }

        [UriFormat("/wallmount")]
        public IGetResponse GetAll()
        {
            try
            {
                var wallmounts = _wallmountSwitchManager.Get();
                return ResponseUtilities.GetResponseOk(wallmounts);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not get wallmounts: exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount/{deviceId}")]
        public IGetResponse GetById(string deviceId)
        {
            try
            {
                var wallmount = _wallmountSwitchManager.Get(deviceId);
                return ResponseUtilities.GetResponseOk(wallmount);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not get wallmount with deviceid: {deviceId} , exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount")]
        public async Task<IPostResponse> Add([FromContent] WallMountSwitchDto dto)
        {
            try
            {
                if (Common.Validation.ValidateDeviceId(dto.DeviceId))
                {
                    throw new ArgumentException(nameof(dto.DeviceId));
                }

                var wallmount = await _wallmountSwitchManager.Add(dto);
                return ResponseUtilities.PostResponseOk(wallmount);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not add wallmount, exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount")]
        public IPutResponse Update([FromContent] WallMountSwitchDto dto)
        {
            try
            {
                var wallmount = _wallmountSwitchManager.Update(dto);
                return ResponseUtilities.PutResponseOk(wallmount);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not edit wallmount switch: exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount/{deviceId}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            try
            {
                await _wallmountSwitchManager.Remove(deviceId);
                return ResponseUtilities.DeleteResponseOk();
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not remove wallmount: exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount/{deviceId}/testconnection")]
        public IGetResponse TestConnection(string deviceId)
        {
            try
            {
                bool succeeded = _wallmountSwitchManager.TestConnection(deviceId);

                return ResponseUtilities.GetResponseOk(succeeded);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception(ex.Message);
            }
        }

        [UriFormat("/wallmount/{deviceId}/state")]
        public IGetResponse GetState(string deviceId)
        {
            try
            {
                var isOn = _wallmountSwitchManager.IsOn(deviceId);
                return ResponseUtilities.GetResponseOk(isOn.ToString());
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not determine wallmount state: exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount/{deviceId}/switch/{state}")]
        public IPutResponse Switch(string deviceId, string state)
        {
            try
            {
                //Send switch command to the manager
                _wallmountSwitchManager.Switch(deviceId, state);

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk("ZWave command was sent");
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Wallmount switch failed, exception: {ex.Message}");
            }
        }
    }
}
