using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.WebServer.Attributes;
using System;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize("Manager")]
    [RestController]
    public class DanaLockController
    {
        private readonly IDanaLockManager _danaLockManager;

        public DanaLockController(IDanaLockManager danaLockManager)
        {
            _danaLockManager = danaLockManager;
        }

        [UriFormat("/danalock")]
        public IGetResponse GetAll()
        {
            try
            {
                var locks = _danaLockManager.Get();
                return ResponseUtilities.GetResponseOk(locks);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not get danaLocks: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock")]
        public async Task<IPostResponse> Add([FromContent] DanaLockDto dto)
        {
            try
            {
                if (Common.Validation.ValidateDeviceId(dto.DeviceId))
                {
                    throw new ArgumentException(nameof(dto.DeviceId));
                }

                var danalock = await _danaLockManager.Add(dto);
                return ResponseUtilities.PostResponseOk(danalock);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not add danalock: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock")]
        public IPutResponse Edit([FromContent] DanaLockDto dto)
        {
            try
            {
                var danalock = _danaLockManager.Update(dto);
                return ResponseUtilities.PutResponseOk(danalock);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not edit danaLock: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/{deviceId}")]
        public async Task<IDeleteResponse> Delete(string deviceId)
        {
            try
            {
                await _danaLockManager.Remove(deviceId);
                return ResponseUtilities.DeleteResponseOk();
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not remove danaLock: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/{deviceId}/testconnection")]
        public IGetResponse TestConnection(string deviceId)
        {
            try
            {
                bool succeeded = _danaLockManager.TestConnection(deviceId);
                return ResponseUtilities.GetResponseOk(succeeded);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception(ex.Message);
            }
        }

        [UriFormat("/danalock/{deviceId}/islocked")]
        public IGetResponse IsLocked(string deviceId)
        {
            try
            {
                var isLocked = _danaLockManager.IsLocked(deviceId);
                return ResponseUtilities.GetResponseOk(isLocked.ToString());
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not determine danalock status: exception: {ex.Message}");
            }
        }

        [UriFormat("/danalock/{deviceId}/switch/{state}")]
        public IPutResponse Switch(string deviceId, string state)
        {
            try
            {
                //Send switch command to the manager
                _danaLockManager.Switch(deviceId, state);

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk("ZWave command was sent");
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"DanaLock switch failed, exception: {ex.Message}");
            }
        }
    }
}
