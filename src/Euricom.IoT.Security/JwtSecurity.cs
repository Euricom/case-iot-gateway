using JoseRT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Security
{
    public static class JwtSecurity
    {
        private static int _tokenExpiryInMinutes = 60;
        // TODO change key
        private static byte[] _secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };

        public static string GenerateJwt(string username)
        {
            var now = DateTimeOffset.Now;
            var iat = now.ToUnixTimeSeconds();
            var payload = JsonConvert.SerializeObject(new JwtPayload()
            {
                iss = "IoT Gateway",
                iat = now.ToUnixTimeSeconds(),
                exp = now.AddMinutes(_tokenExpiryInMinutes).ToUnixTimeSeconds(),
                sub = username
            });
            string jwt = JoseRT.Jwt.Encode(payload, JwsAlgorithms.HS256, _secretKey);
            return jwt;
        }

        public static JwtPayload DecodeJwt(string jwt)
        {
            string decoded = JoseRT.Jwt.Decode(jwt, _secretKey);
            return JsonConvert.DeserializeObject<JwtPayload>(decoded);
        }

        public static bool VerifyJwt(string jwt)
        {
            try
            {
                var payload = DecodeJwt(jwt);

                // Check if token is not yet expired
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(payload.exp).DateTime;
                if (expirationDate <= DateTime.UtcNow)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool VerifyAccessTokenJwt(string accessToken)
        {
            try
            {
                // TODO verify if accessToken is valid
                // TODO check exp time
                DecodeJwt(accessToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public class JwtPayload
        {
            public string iss { get; set; }
            public long iat { get; set; }
            public long exp { get; set; }
            public string sub { get; set; }
        }
    }
}
