using System;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Logging;
using Euricom.IoT.Models.Security;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.WebServer.Attributes;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
    [RestController]
    public class CommandTokenController
    {
        private readonly ISecurityManager _securityManager;

        public CommandTokenController(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        [UriFormat("/security/command-token")]
        public IPostResponse RequestCommandToken([FromContent] RequestForAccessToken request)
        {
            try
            {
                var commandTokenJwt = _securityManager.RequestCommandToken(request.AccessToken);
                return ResponseUtilities.PostResponseOk(commandTokenJwt);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not get command token: exception: {ex.Message}");
            }
        }
    }
}