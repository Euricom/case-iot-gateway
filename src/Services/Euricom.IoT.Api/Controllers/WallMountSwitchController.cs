using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
    [RestController(InstanceCreationType.Singleton)]
    public class WallMountSwitchController
    {
        private readonly Euricom.IoT.Api.Managers.Interfaces.IWallMountSwitchManager _wallmountSwitchManager;

        public WallMountSwitchController()
        {

        }
    }
}
