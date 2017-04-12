using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Logging;
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
    public class LazyBoneController
    {
        private readonly ILazyBoneManager _lazyBoneManager;

        public LazyBoneController()
        {
            _lazyBoneManager = new LazyBoneManager();
        }

        [UriFormat("/lazybone")]
        public async Task<IGetResponse> GetAll()
        {
            try
            {
                var lazyBones = await _lazyBoneManager.GetAll();
                var lazyBonesDto = Mapper.Map<IEnumerable<LazyBoneDto>>(lazyBones);
                return ResponseUtilities.GetResponseOk(lazyBonesDto);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.GetResponseFail($"Could not get lazyBones: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/{devicename}")]
        public async Task<IGetResponse> GetByDeviceName(string devicename)
        {
            try
            {
                var lazyBone = await _lazyBoneManager.GetByDeviceName(devicename);
                var lazyBoneDto = Mapper.Map<LazyBoneDto>(lazyBone);
                return ResponseUtilities.GetResponseOk(lazyBoneDto);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(devicename, ex);
                return ResponseUtilities.GetResponseFail($"Could not get lazybone with devicename {devicename} , exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone")]
        public async Task<PostResponse> Add([FromContent] LazyBoneDto lazyBoneDto)
        {
            try
            {
                var lazyBone = Mapper.Map<LazyBone>(lazyBoneDto);
                var newLazyBone = await _lazyBoneManager.Add(lazyBone);
                return ResponseUtilities.PostResponseOk(newLazyBone);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.PostResponseFail($"Could not add lazyBone: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone")]
        public async Task<IPutResponse> Edit([FromContent] LazyBoneDto lazyBoneDto)
        {
            try
            {
                var lazyBone = Mapper.Map<LazyBone>(lazyBoneDto);
                var lazyBoneEdited = await _lazyBoneManager.Edit(lazyBone);
                return ResponseUtilities.PutResponseOk(lazyBoneEdited);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.PutResponseFail($"Could not edit lazyBone: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/{devicename}")]
        public async Task<IDeleteResponse> Delete(string devicename)
        {
            try
            {
                var removed = await _lazyBoneManager.Remove(devicename);
                return ResponseUtilities.DeleteResponseOk(removed.ToString());
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(devicename, ex);
                return ResponseUtilities.DeleteResponseFail($"Could not remove lazyBone: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/testconnection/{devicename}")]
        public async Task<IGetResponse> TestConnection(string devicename)
        {
            try
            {
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                string softwareVersion = await _lazyBoneManager.TestConnection(deviceId);
                return ResponseUtilities.GetResponseOk(softwareVersion);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(devicename, ex);
                return ResponseUtilities.GetResponseFail(ex.Message);
            }
        }

        [UriFormat("/lazybone/getstate/{devicename}")]
        public async Task<IGetResponse> GetState(string devicename)
        {
            try
            {
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                var isRelayOn = await _lazyBoneManager.GetCurrentState(deviceId);
                return ResponseUtilities.GetResponseOk(isRelayOn.ToString());
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(devicename, ex);
                return ResponseUtilities.GetResponseFail($"Could not determine danalock status: exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends a switch command to a specific LazyBone device 
        /// </summary>
        /// <param name="device">Guid of device</param>
        /// <param name="state">on or off</param>
        /// <returns></returns>
        [UriFormat("/lazybone/switch?devicename={devicename}&state={state}")]
        public async Task<IPutResponse> Switch(string devicename, string state)
        {
            try
            {
                //Send switch command to the manager
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                await _lazyBoneManager.Switch(deviceId, state);

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk($"OK changed LazyBone device state to : {state}");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(devicename, ex);
                return ResponseUtilities.PutResponseFail($"LazyBone switch failed, exception: {ex.Message}");
            }
        }
    }
}
