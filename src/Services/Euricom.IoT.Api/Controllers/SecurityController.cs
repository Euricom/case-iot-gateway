using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Logging;
using Euricom.IoT.Mailing;
using Euricom.IoT.Models.Security;
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
            _securityManager = new SecurityManager(new Mailer());
        }

        [UriFormat("/security/login")]
        public IPostResponse Login([FromContent] LoginCredentials credentials)
        {
            try
            {
                var jwt = _securityManager.Login(credentials.Username, credentials.Password);
                Logger.Instance.LogInformationWithContext(this.GetType(), $"{credentials.Username} logged in");
                return ResponseUtilities.PostResponseOk(jwt);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not login: exception: {ex.Message}");
            }
        }

        [UriFormat("/security/loginByPUK")]
        public IPostResponse Login([FromContent] PukCredentials credentials)
        {
            try
            {
                var jwt = _securityManager.LoginWithPUK(credentials.PUK);
                Logger.Instance.LogInformationWithContext(this.GetType(), "admin logged in with PUK code!");
                return ResponseUtilities.PostResponseOk(jwt);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not login: exception: {ex.Message}");
            }
        }

        [UriFormat("/security/getcommandtoken")]
        public IPostResponse RequestCommandToken([FromContent] RequestForAccessToken request)
        {
            try
            {
                var commandTokenJwt = _securityManager.RequestCommandToken(request.JWT);
                return ResponseUtilities.PostResponseOk(commandTokenJwt);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not get log: exception: {ex.Message}");
            }
        }
    }
}
