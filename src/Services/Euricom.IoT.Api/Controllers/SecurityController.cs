using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Logging;
using Euricom.IoT.Models.Security;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using Euricom.IoT.Api.Models;
using Restup.WebServer.Attributes;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.PerCall)]
    public class SecurityController: ControllerBase
    {
        private readonly ISecurityManager _securityManager;

        public SecurityController(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        [UriFormat("/security/login")]
        public IPostResponse Login([FromContent] LoginCredentials credentials)
        {
            try
            {
                var jwt = _securityManager.Login(credentials.Username, credentials.Password);
                Logger.Instance.LogInformationWithContext(GetType(), $"{credentials.Username} logged in");
                return ResponseUtilities.PostResponseOk(jwt);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not login: exception: {ex.Message}");
            }
        }

        [UriFormat("/security/loginByPUK")]
        public IPostResponse Login([FromContent] PukCredentials credentials)
        {
            try
            {
                var jwt = _securityManager.LoginWithPuk(credentials.PUK);
                Logger.Instance.LogInformationWithContext(GetType(), "admin logged in with PUK code!");
                return ResponseUtilities.PostResponseOk(jwt);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not login: exception: {ex.Message}");
            }
        }

        [UriFormat("/security/requestcommandtoken")]
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
                throw new Exception($"Could not get log: exception: {ex.Message}");
            }
        }

        [Authorize]
        [UriFormat("/security/password")]
        public IPutResponse ChangePassword([FromContent] ChangePasswordDto dto)
        {
            try
            {
                var username = GetUsername();
                _securityManager.ChangePassword(username, dto.Old, dto.New);

                Logger.Instance.LogInformationWithContext(GetType(), $"{username} changed password!");

                return new PutResponse(PutResponse.ResponseStatus.OK);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not change password: exception: {ex.Message}");
            }
        }
    }
}
