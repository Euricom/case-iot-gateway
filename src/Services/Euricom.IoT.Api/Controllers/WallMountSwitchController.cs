using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;
using System;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
    [RestController(InstanceCreationType.Singleton)]
    public class WallMountSwitchController
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
        public IPostResponse Add([FromContent] WallMountSwitchDto wallmountSwitchDto)
        {
            try
            {
                var newWallMount = _wallmountSwitchManager.Add(wallmountSwitchDto);
                return ResponseUtilities.PostResponseOk(newWallMount);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not add wallmount, exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount")]
        public IPutResponse Update([FromContent] WallMountSwitchDto wallmountSwitchDto)
        {
            try
            {
                var wallmountSwitchEdited = _wallmountSwitchManager.Update(wallmountSwitchDto);
                return ResponseUtilities.PutResponseOk(wallmountSwitchEdited);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not edit wallmount switch: exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount/{deviceId}")]
        public IDeleteResponse Delete(string deviceId)
        {
            try
            {
                _wallmountSwitchManager.Remove(deviceId);
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
