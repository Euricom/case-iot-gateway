using Euricom.IoT.Api.Manager;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class CameraController
    {
        private readonly ICameraManager _cameraManager;

        public CameraController()
        {
            _cameraManager = new CameraManager();
        }

        public IGetResponse Add(Camera camera)
        {
            try
            {
                _cameraManager.Add(camera);
                return new GetResponse(GetResponse.ResponseStatus.OK, camera);
            }
            catch (Exception ex)
            {
                //TODO add logging
                throw;
            }
        }

        [UriFormat("/camera/notify?deviceid={deviceid}&url={url}&ts={timestamp}&frame={frameNumber}&event={eventNumber}")]
        public IGetResponse Notify(string deviceid, string url, string timestamp, int frameNumber, int eventNumber)
        {
            //Send notification to IoT hub
            _cameraManager.Notify(deviceid, url, timestamp, frameNumber, eventNumber);

            // Send response back
            return new GetResponse(GetResponse.ResponseStatus.OK);
        }
    }
}
