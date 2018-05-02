using System;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Logging;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;

namespace Euricom.IoT.Api.Controllers
{
    [RestController]
    public class CommandTokenController
    {
        private readonly ISecurityManager _securityManager;

        public CommandTokenController(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        [UriFormat("/security/command-token")]
        public IPostResponse RequestCommandToken([FromContent] GetCommandTokenDto request)
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