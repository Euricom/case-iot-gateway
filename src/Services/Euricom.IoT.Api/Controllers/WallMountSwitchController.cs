using AutoMapper;
using Euricom.IoT.Api.Dtos;
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
    public class WallMountSwitchController
    {
        private readonly IWallMountSwitchManager _wallmountSwitchManager;

        public WallMountSwitchController()
        {
            _wallmountSwitchManager = new WallMountSwitchManager();
        }

        [UriFormat("/wallmount")]
        public async Task<IGetResponse> GetAll()
        {
            try
            {
                var wallmounts = await _wallmountSwitchManager.GetAll();
                var danaLocksDto = Mapper.Map<IEnumerable<WallMountSwitchDto>>(wallmounts);
                return ResponseUtilities.GetResponseOk(danaLocksDto);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not get wallmounts: exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount/{devicename}")]
        public async Task<IGetResponse> GetByDeviceName(string devicename)
        {
            try
            {
                var wallmount = await _wallmountSwitchManager.GetByDeviceName(devicename);
                var wallmountSwitchDto = Mapper.Map<WallMountSwitchDto>(wallmount);
                return ResponseUtilities.GetResponseOk(wallmountSwitchDto);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not get wallmount with devicename: {devicename} , exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount")]
        public async Task<IPostResponse> Add([FromContent] WallMountSwitchDto wallmountSwitchDto)
        {
            try
            {
                var wallmount = Mapper.Map<WallMountSwitch>(wallmountSwitchDto);
                var newWallMount = await _wallmountSwitchManager.Add(wallmount);
                return ResponseUtilities.PostResponseOk(newWallMount);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not add wallmount, exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount")]
        public async Task<IPutResponse> Edit([FromContent] WallMountSwitchDto wallmountSwitchDto)
        {
            try
            {
                var wallmountSwitch = Mapper.Map<WallMountSwitch>(wallmountSwitchDto);
                var wallmountSwitchEdited = await _wallmountSwitchManager.Edit(wallmountSwitch);
                return ResponseUtilities.PutResponseOk(wallmountSwitchEdited);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not edit wallmount switch: exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount/{devicename}")]
        public async Task<IDeleteResponse> Delete(string devicename)
        {
            try
            {
                await _wallmountSwitchManager.Remove(devicename);
                return ResponseUtilities.DeleteResponseOk();
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not remove wallmount: exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount/testconnection/{devicename}")]
        public async Task<IGetResponse> TestConnection(string devicename)
        {
            try
            {
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                bool succeeded = _wallmountSwitchManager.TestConnection(deviceId);
                return ResponseUtilities.GetResponseOk(succeeded);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception(ex.Message);
            }
        }

        [UriFormat("/wallmount/getState/{devicename}")]
        public async Task<IGetResponse> GetState(string devicename)
        {
            try
            {
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                var isOn = await _wallmountSwitchManager.IsOn(deviceId);
                return ResponseUtilities.GetResponseOk(isOn.ToString());
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not determine wallmount state: exception: {ex.Message}");
            }
        }

        [UriFormat("/wallmount/switch?devicename={devicename}&state={state}")]
        public async Task<IPutResponse> Switch(string devicename, string state)
        {
            try
            {
                //Send switch command to the manager
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                await _wallmountSwitchManager.Switch(deviceId, state);

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk($"ZWave command was sent");
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Wallmount switch failed, exception: {ex.Message}");
            }
        }
    }
}
