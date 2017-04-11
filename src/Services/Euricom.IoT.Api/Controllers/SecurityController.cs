using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common.Security;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class SecurityController
    {
        private readonly ISecurityManager _securityManager;

        public SecurityController()
        {
            _securityManager = new SecurityManager();
        }

        [UriFormat("/security/getcommandtoken")]
        public IPostResponse RequestCommandToken([FromContent] RequestForAccessToken request)
        {
            try
            {
                var logFiles = _securityManager.RequestCommandToken(request.JWT);
                return ResponseUtilities.PostResponseOk(logFiles);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.PostResponseFail($"Could not get log: exception: {ex.Message}");
            }
        }
    }
}
