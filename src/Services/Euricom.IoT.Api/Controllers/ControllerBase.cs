using System.Linq;
using Euricom.IoT.Security;
using Restup.WebServer.Rest;

namespace Euricom.IoT.Api.Controllers
{
    public class ControllerBase : RestControllerBase
    {
        public string GetUsername()
        {
            var auth = Request.Headers.SingleOrDefault(h => h.Name == "Authorization");

            if (auth == null)
            {
                return null;
            }

            var jwt = auth.Value.Replace("Bearer ", "");
            var claims = JwtSecurity.DecodeJwt(jwt);

            return claims.sub;
        }
    }
}