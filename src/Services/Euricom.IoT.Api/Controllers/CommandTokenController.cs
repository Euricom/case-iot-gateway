using System;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Api.Utilities;
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
            if (request == null || string.IsNullOrEmpty(request.AccessToken))
            {
                throw new ArgumentException();
            }

            var commandTokenJwt = _securityManager.RequestCommandToken(request.AccessToken);
            return ResponseUtilities.PostResponseOk(commandTokenJwt);
        }
    }
}