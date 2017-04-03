using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
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

        [UriFormat("/danalock/add")]
        public async Task<IPostResponse> Add([FromContent] Common.DanaLock danaLock)
        {
            try
            {
                var newDanaLock = await _danaLockManager.Add(danaLock);
                return ResponseUtilities.PostResponseOk(newDanaLock.DeviceId);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.PostResponseFail($"Could not determine danalock status: exception: {ex.Message}");
            }
        }

        public async Task<IPutResponse> Edit(Common.DanaLock danaLock)
        {
            try
            {
                var danaLockEdited = await _danaLockManager.Edit(danaLock);
                return ResponseUtilities.PutResponseOk(danaLockEdited);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.PutResponseFail($"Could not edit danaLock: exception: {ex.Message}");
            }
        }

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

        [UriFormat("/danalock/islocked?deviceid={deviceid}")]
        public IGetResponse IsLocked(string deviceid)
        {
            try
            {
                var isLocked = _danaLockManager.IsLocked(deviceid);
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
