using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using Euricom.IoT.Api.Managers;
using System.Threading.Tasks;
using System.Diagnostics;

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

        public PostResponse Add(LazyBone lazyBone)
        {
            try
            {
                var added = _lazyBoneManager.Add(lazyBone);
                return ResponseUtilities.PostResponseOk(added.DeviceId);
            }
            catch (Exception ex)
            {
                return ResponseUtilities.PostResponseFail($"Could not determine danalock status: exception: {ex.Message}");
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
                Debug.WriteLine("LazyBoneController: Switch()");

                //Send switch command to the manager
                await _lazyBoneManager.Switch(deviceid, state);

                Debug.WriteLine("LazyBoneController: Switch() completed");

                //If it works, send response back to client
                return ResponseUtilities.PutResponseOk($"OK changed LazyBone device state to : {state}");
            }
            catch (Exception ex)
            {
                return ResponseUtilities.PutResponseFail($"LazyBone switch failed, exception: {ex.Message}");
            }
        }


    }
}
