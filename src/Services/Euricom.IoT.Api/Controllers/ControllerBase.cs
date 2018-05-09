using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common.Exceptions;
using Restup.WebServer.Rest;

namespace Euricom.IoT.Api.Controllers
{
    public class ControllerBase : RestControllerBase
    {
        protected string GetUsername()
        {
            var username = Request.GetUsername();

            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedException();
            }

            return username;
        }
    }
}