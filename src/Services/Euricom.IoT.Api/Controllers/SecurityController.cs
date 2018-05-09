using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Models.Security;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Logging;
using Restup.WebServer.Attributes;

namespace Euricom.IoT.Api.Controllers
{
    [RestController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityManager _securityManager;

        public SecurityController(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        [UriFormat("/security/login")]
        public IPostResponse Login([FromContent] LoginCredentials credentials)
        {
            var jwt = _securityManager.Login(credentials.Username, credentials.Password);

            Logger.Instance.Information($"{credentials.Username} logged in");

            return ResponseUtilities.PostResponseOk(jwt);
        }

        [UriFormat("/security/loginByPUK")]
        public IPostResponse Login([FromContent] PukCredentials credentials)
        {
            var jwt = _securityManager.LoginWithPuk(credentials.PUK);

            Logger.Instance.Information("admin logged in with PUK code!");

            return ResponseUtilities.PostResponseOk(jwt);
        }

        [Authorize]
        [UriFormat("/security/password")]
        public IPutResponse ChangePassword([FromContent] ChangePasswordDto dto)
        {
            var username = GetUsername();

            _securityManager.ChangePassword(username, dto.Old, dto.New);

            Logger.Instance.Information($"{username} changed password!");

            return new PutResponse(PutResponse.ResponseStatus.NoContent);
        }
    }
}
