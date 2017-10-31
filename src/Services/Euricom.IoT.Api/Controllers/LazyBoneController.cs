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
using Euricom.IoT.DataLayer;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
    [RestController(InstanceCreationType.Singleton)]
    public class LazyBoneController
    {
        private readonly Database _database;
        private readonly ILazyBoneManager _lazyBoneManager;

        public LazyBoneController(Database database, ILazyBoneManager lazyBoneManager)
        {
            _database = database;
            _lazyBoneManager = lazyBoneManager;
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
                throw new Exception($"Could not get lazyBones: exception: {ex.Message}");
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
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not get lazybone with devicename {devicename} , exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone")]
        public async Task<PostResponse> Add([FromContent] LazyBoneDto lazyBoneDto)
        {
            try
            {
                var lazyBone = Mapper.Map<IoT.Models.LazyBone>(lazyBoneDto);
                var newLazyBone = await _lazyBoneManager.Add(lazyBone);
                return ResponseUtilities.PostResponseOk(newLazyBone);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not add lazyBone: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone")]
        public async Task<IPutResponse> Edit([FromContent] LazyBoneDto lazyBoneDto)
        {
            try
            {
                var lazyBone = Mapper.Map<IoT.Models.LazyBone>(lazyBoneDto);
                var lazyBoneEdited = await _lazyBoneManager.Edit(lazyBone);
                return ResponseUtilities.PutResponseOk(lazyBoneEdited);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not edit lazyBone: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/{devicename}")]
        public async Task<IDeleteResponse> Delete(string devicename)
        {
            try
            {
                await _lazyBoneManager.Remove(devicename);
                return ResponseUtilities.DeleteResponseOk();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not remove lazyBone: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/testconnection/{devicename}")]
        public async Task<IGetResponse> TestConnection(string devicename)
        {
            try
            {
                var deviceId = _lazyBoneManager.GetDeviceId(devicename);
                bool connectionSuccessfull = await _lazyBoneManager.TestConnection(deviceId);
                return ResponseUtilities.GetResponseOk(connectionSuccessfull);
            }
            catch (Exception ex)
            {
                var deviceId = _lazyBoneManager.GetDeviceId(devicename);
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw new Exception(ex.Message);
            }
        }

        [UriFormat("/lazybone/getstate/{devicename}")]
        public async Task<IGetResponse> GetState(string devicename)
        {
            try
            {
                var deviceId = _lazyBoneManager.GetDeviceId(devicename);
                var config = _database.GetLazyBoneConfig(deviceId);
                if (!config.IsDimmer)
                {
                    var isRelayOn = await _lazyBoneManager.GetCurrentStateSwitch(deviceId);
                    return ResponseUtilities.GetResponseOk(isRelayOn.ToString());
                }
                else
                {
                    var dimmerState = await _lazyBoneManager.GetCurrentStateDimmer(deviceId);
                    return ResponseUtilities.GetResponseOk(dimmerState.ToString());
                }
            }
            catch (Exception ex)
            {
                var deviceId = _lazyBoneManager.GetDeviceId(devicename);
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw new Exception($"Could not determine lazybone state: exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/switch?devicename={devicename}&state={state}")]
        public async Task<IPutResponse> Switch(string devicename, string state)
        {
            try
            {
                //Send switch command to the manager
                var deviceId = _lazyBoneManager.GetDeviceId(devicename);
                await _lazyBoneManager.Switch(deviceId, state);

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk($"LazyBone switched state to : {state}");
            }
            catch (Exception ex)
            {
                var deviceId = _lazyBoneManager.GetDeviceId(devicename);
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw new Exception($"LazyBone switch failed, exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/setlightvalue?devicename={devicename}&value={value}")]
        public async Task<IPutResponse> SetLightValue(string devicename, short value)
        {
            try
            {
                //Send switch command to the manager
                var deviceId = _lazyBoneManager.GetDeviceId(devicename);
                await _lazyBoneManager.SetLightValue(deviceId, value);

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk($"LazyBone dimmer changed light value to : {value}");
            }
            catch (Exception ex)
            {
                var deviceId = _lazyBoneManager.GetDeviceId(devicename);
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw new Exception($"LazyBone dimmer failed, exception: {ex.Message}");
            }
        }

        [UriFormat("/lazybone/testchangelightintensity?devicename={devicename}")]
        public async Task<IPutResponse> TestChangeLightIntensity(string devicename)
        {
            try
            {
                //Send switch command to the manager
                var deviceId = _lazyBoneManager.GetDeviceId(devicename);
                await _lazyBoneManager.TestChangeLightIntensity(deviceId);

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk($"LazyBone dimmer changed light values 3 times");
            }
            catch (Exception ex)
            {
                var deviceId = _lazyBoneManager.GetDeviceId(devicename);
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw new Exception($"LazyBone dimmer failed, exception: {ex.Message}");
            }
        }
    }
}
