using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class HardwareController
    {
        private readonly IHardwareManager _hardwareManager;

        public HardwareController()
        {
            _hardwareManager = new HardwareManager();
        }

        public IGetResponse GetHardware()
        {
            var hardware = _hardwareManager.GetHardware();
            return new GetResponse(GetResponse.ResponseStatus.OK, hardware);
        }

        public IPostResponse AddHardware(string name, string type)
        {
            _hardwareManager.AddHardware(name, type);
            return new PostResponse(PostResponse.ResponseStatus.Created);
        }
    }
}
