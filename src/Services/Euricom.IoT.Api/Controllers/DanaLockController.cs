using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

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

        public void Add(DanaLock danaLock)
        {
            _danaLockManager.Add(danaLock);
        }

        //[UriFormat("/danalock\\?deviceid={deviceid}&state={state}")]
        //[UriFormat("/danalock/device/{deviceId}/state/{state}")]
        [UriFormat("/danalock?deviceid={deviceid}&state={state}")]
        public IGetResponse Switch(string deviceid, string state)
        {
            try
            {
                //Send switch command to the manager
                _danaLockManager.Switch(deviceid, state);

                //If it works, send response back to client
                return ResponseUtilities.ResponseOk($"OK changed DanaLock device state to : {state}");
            }
            catch (Exception ex)
            {
                return ResponseUtilities.ResponseFail($"DanaLock failed, exception: {ex.Message}");
            }
        }
    }
}
