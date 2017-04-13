using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Restup.HttpMessage;
using Restup.HttpMessage.Models.Schemas;
using Restup.Webserver.Models.Contracts;
using Restup.WebServer.Models.Contracts;
using Restup.WebServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restup.WebServer.Http
{
    public class JwtAuthenticationProvider : IAuthorizationProvider
    {
        private readonly ISecurityManager _securityManager;

        public string Realm { get { return null; } }

        //ICredentialValidator is not used
        public JwtAuthenticationProvider(string realm, ICredentialValidator credentialValidator)
        {
            _securityManager = new SecurityManager(new Euricom.IoT.Mailing.Mailer());
        }

        public HttpResponseStatus Authorize(IHttpServerRequest request)
        {
            if (request.Headers.Any(h => h.Name == "Authorization"))
            {
                // Get jwt token
                var authValue = request.Headers.Single(h => h.Name == "Authorization").Value;
                var jwtToken = authValue.Replace("Bearer ", "");

                // Validate token
                var valid = _securityManager.ValidateToken(jwtToken);

                // If not a valid token return unauthorized
                if (!valid)
                    return HttpResponseStatus.Unauthorized;
                // Else return Http status code 200
                else
                    return HttpResponseStatus.OK;
            }
            else // ask for authentication headers
            {
                return HttpResponseStatus.Unauthorized;
            }
        }
    }
}
