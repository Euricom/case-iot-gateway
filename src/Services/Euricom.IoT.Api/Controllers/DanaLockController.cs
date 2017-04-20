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
    public class DanaLockController
    {
        private readonly IDanaLockManager _danaLockManager;

        public DanaLockController()
        {
            _danaLockManager = new DanaLockManager();
        }

        [UriFormat("/danalock")]
        public async Task<IGetResponse> GetAll()
        {
            try
            {
                var danaLocks = await _danaLockManager.GetAll();
                var danaLocksDto = Mapper.Map<IEnumerable<DanaLockDto>>(danaLocks);
                return ResponseUtilities.GetResponseOk(danaLocksDto);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not get danaLocks: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/{devicename}")]
        public async Task<IGetResponse> GetByDeviceName(string devicename)
        {
            try
            {
                var danaLock = await _danaLockManager.GetByDeviceName(devicename);
                var danaLockDto = Mapper.Map<DanaLockDto>(danaLock);
                return ResponseUtilities.GetResponseOk(danaLockDto);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not get danaLock with devicename: {devicename} , exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock")]
        public async Task<IPostResponse> Add([FromContent] DanaLockDto danaLockDto)
        {
            try
            {
                var danaLock = Mapper.Map<DanaLock>(danaLockDto);
                var newLazyBone = await _danaLockManager.Add(danaLock);
                return ResponseUtilities.PostResponseOk(newLazyBone);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not add danalock: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock")]
        public async Task<IPutResponse> Edit([FromContent] DanaLockDto danaLockDto)
        {
            try
            {
                var danaLock = Mapper.Map<DanaLock>(danaLockDto);
                var danaLockEdited = await _danaLockManager.Edit(danaLock);
                return ResponseUtilities.PutResponseOk(danaLockEdited);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not edit danaLock: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/{devicename}")]
        public async Task<IDeleteResponse> Delete(string devicename)
        {
            try
            {
                await _danaLockManager.Remove(devicename);
                return ResponseUtilities.DeleteResponseOk();
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not remove danaLock: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/testconnection/{devicename}")]
        public async Task<IGetResponse> TestConnection(string devicename)
        {
            try
            {
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                bool succeeded = _danaLockManager.TestConnection(deviceId);
                return ResponseUtilities.GetResponseOk(succeeded);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception(ex.Message);
            }
        }

        [UriFormat("/danalock/islocked/{devicename}")]
        public async Task<IGetResponse> IsLocked(string devicename)
        {
            try
            {
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                var isLocked = await _danaLockManager.IsLocked(deviceId);
                return ResponseUtilities.GetResponseOk(isLocked.ToString());
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not determine danalock status: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/switch?devicename={devicename}&state={state}")]
        public async Task<IPutResponse> Switch(string devicename, string state)
        {
            try
            {
                //Send switch command to the manager
                var deviceId = new HardwareManager().GetDeviceId(devicename);
                await _danaLockManager.Switch(deviceId, state);

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk($"ZWave command was sent");
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"DanaLock switch failed, exception: {ex.Message}");
            }
        }
    }
}
