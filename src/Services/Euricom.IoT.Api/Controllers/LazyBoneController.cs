using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Euricom.IoT.Logging;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Controllers
{
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

        [UriFormat("/lazybone/{deviceid}")]
        public async Task<IGetResponse> GetByDeviceId(string deviceid)
        {
            try
            {
                var lazyBone = await _lazyBoneManager.Get(deviceid);
                var lazyBoneDto = Mapper.Map<LazyBoneDto>(lazyBone);
                return ResponseUtilities.GetResponseOk(lazyBoneDto);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceid, ex);
                return ResponseUtilities.GetResponseFail($"Could not get lazybone with deviceId {deviceid} , exception: {ex.Message}");
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

        [UriFormat("/lazybone/{deviceid}")]
        public async Task<IDeleteResponse> Delete(string deviceid)
        {
            try
            {
                var removed = await _lazyBoneManager.Remove(deviceid);
                return ResponseUtilities.DeleteResponseOk(removed.ToString());
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceid, ex);
                return ResponseUtilities.DeleteResponseFail($"Could not remove lazyBone: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/testconnection/{deviceid}")]
        public async Task<IGetResponse> TestConnection(string deviceid)
        {
            try
            {
                string softwareVersion = await _lazyBoneManager.TestConnection(deviceid);
                return ResponseUtilities.GetResponseOk(softwareVersion);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceid, ex);
                return ResponseUtilities.GetResponseFail(ex.Message);
            }
        }

        [UriFormat("/lazybone/getstate/{deviceid}")]
        public async Task<IGetResponse> GetState(string deviceid)
        {
            try
            {
                var isRelayOn = await _lazyBoneManager.GetCurrentState(deviceid);
                return ResponseUtilities.GetResponseOk(isRelayOn.ToString());
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceid, ex);
                return ResponseUtilities.GetResponseFail($"Could not determine danalock status: exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends a switch command to a specific LazyBone device 
        /// </summary>
        /// <param name="device">Guid of device</param>
        /// <param name="state">on or off</param>
        /// <returns></returns>
        [UriFormat("/lazybone/switch?deviceid={deviceid}&state={state}")]
        public async Task<IPutResponse> Switch(string deviceid, string state)
        {
            try
            {
                throw new Exception("test van wim");

                Debug.WriteLine("LazyBoneController: Switch()");

                //Send switch command to the manager
                await _lazyBoneManager.Switch(deviceid, state);

                Debug.WriteLine("LazyBoneController: Switch() completed");

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk($"OK changed LazyBone device state to : {state}");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceid, ex);
                return ResponseUtilities.PutResponseFail($"LazyBone switch failed, exception: {ex.Message}");
            }
        }
    }
}
