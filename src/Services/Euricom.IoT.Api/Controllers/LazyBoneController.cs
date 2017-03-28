using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class LazyBoneController
    {
        private readonly ILazyBoneManager _lazyBoneManager;

        public LazyBoneController()
        {
        }

        /// <summary>
        /// Sends a switch command to a specific LazyBone device 
        /// </summary>
        /// <param name="device">Guid of device</param>
        /// <param name="state">on or off</param>
        /// <returns></returns>
        //[UriFormat("/lazybone/{state}")]
        [UriFormat("/lazybone\\?device={device}&state={state}")]
        //public IGetResponse Switch(string state)
        public IGetResponse Switch(string device, string state)
        {
            try
            {
                //Send switch command to the manager
                _lazyBoneManager.Switch(device, state);

                //If it works, send response back to client
                return ResponseUtilities.ResponseOk($"OK changed LazyBone device state to : {state}");
            }
            catch (Exception ex)
            {
                return ResponseUtilities.ResponseFail($"LazyBone switch failed, exception: {ex.Message}");
            }
        }

        internal void Add(LazyBone @switch)
        {
            throw new NotImplementedException();
        }
    }
}
