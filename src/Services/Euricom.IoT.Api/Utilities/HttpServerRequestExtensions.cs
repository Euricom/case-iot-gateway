using System.Linq;
using Euricom.IoT.Security;
using Restup.HttpMessage;

namespace Euricom.IoT.Api.Utilities
{
    public static class HttpServerRequestExtensions
    {
        public static string ToMessageString(this IHttpServerRequest request)
        {
            return $"METHOD: {request.Method}; URI: {request.Uri}; USER: {request.GetUsername()}";
        }

        public static string GetUsername(this IHttpServerRequest request)
        {
            var auth = request.Headers.SingleOrDefault(h => h.Name == "Authorization");

            if (auth == null)
            {
                return string.Empty;
            }

            var jwt = auth.Value.Replace("Bearer ", "");
            var claims = JwtSecurity.DecodeJwt(jwt);

            return claims.Sub;
        }
    }
}