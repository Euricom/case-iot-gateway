using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Controllers
{
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
                return ResponseUtilities.GetResponseFail($"Could not get danaLocks: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/{deviceid}")]
        public async Task<IGetResponse> GetByDeviceId(string deviceid)
        {
            try
            {
                var danaLock = await _danaLockManager.Get(deviceid);
                var danaLockDto = Mapper.Map<DanaLockDto>(danaLock);
                return ResponseUtilities.GetResponseOk(danaLockDto);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.GetResponseFail($"Could not get danaLock with deviceId: {deviceid} , exception: {ex.Message}");
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
                return ResponseUtilities.PostResponseFail($"Could not determine danalock status: exception: {ex.Message}");
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
                return ResponseUtilities.PutResponseFail($"Could not edit danaLock: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/{deviceid}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            try
            {
                var removed = await _danaLockManager.Remove(deviceId);
                return ResponseUtilities.DeleteResponseOk(removed.ToString());
            }
            catch (Exception ex)
            {
                return ResponseUtilities.DeleteResponseFail($"Could not remove danaLock: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/testconnection/{deviceid}")]
        public async Task<IGetResponse> TestConnection(string deviceid)
        {
            try
            {
                bool succeeded = await _danaLockManager.TestConnection(deviceid);
                return ResponseUtilities.GetResponseOk(succeeded);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.GetResponseFail(ex.Message);
            }
        }

        [UriFormat("/danalock/islocked/{deviceid}")]
        public async Task<IGetResponse> IsLocked(string deviceid)
        {
            try
            {
                var isLocked = await _danaLockManager.IsLocked(deviceid);
                return ResponseUtilities.GetResponseOk(isLocked.ToString());
            }
            catch (Exception ex)
            {
                return ResponseUtilities.GetResponseFail($"Could not determine danalock status: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/switch?deviceid={deviceid}&state={state}")]
        public async Task<IPutResponse> Switch(string deviceid, string state)
        {
            try
            {
                //Send switch command to the manager
                await _danaLockManager.Switch(deviceid, state);

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk($"OK DanaLock switched state to : {state}");
            }
            catch (Exception ex)
            {
                return ResponseUtilities.PutResponseFail($"DanaLock switch failed, exception: {ex.Message}");
            }
        }
    }
}
