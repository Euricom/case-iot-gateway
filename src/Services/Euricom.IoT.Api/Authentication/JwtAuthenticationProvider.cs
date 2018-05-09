using System.Collections.Generic;
using System.Linq;
using Euricom.IoT.Api.Managers.Interfaces;
using Restup.HttpMessage;
using Restup.HttpMessage.Models.Schemas;
using Restup.WebServer.Attributes;
using Restup.WebServer.Models.Contracts;

namespace Euricom.IoT.Api.Authentication
{
    public class JwtAuthenticationProvider : IAuthorizationProvider
    {
        private readonly ISecurityManager _securityManager;

        public string Realm => null;

        public JwtAuthenticationProvider(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        public HttpResponseStatus Authorize(IHttpServerRequest request, AuthorizeAttribute attribute = null)
        {
            if (request.Headers.Any(h => h.Name == "Authorization"))
            {
                // Get jwt token
                var authValue = request.Headers.Single(h => h.Name == "Authorization").Value;
                var jwtToken = authValue.Replace("Bearer ", "");

                // Validate token
                var valid = _securityManager.ValidateToken(jwtToken, out var payload);

                // If not a valid token return unauthorized
                if (!valid || attribute != null && ValidateRoles(attribute.Roles, payload.Roles) == false)
                {
                    return HttpResponseStatus.Unauthorized;
                }

                return HttpResponseStatus.OK;
            }

            return HttpResponseStatus.Unauthorized;
        }

        private bool ValidateRoles(List<string> expected, List<string> actual)
        {
            return expected.Any() == false || expected.Intersect(actual).Any();
        }
    }
}
