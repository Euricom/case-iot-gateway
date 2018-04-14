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
        // TODO change key
        private static byte[] _secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };

        public static string GenerateJwt(string username, int expiryInMinutes)
        {
            var now = DateTimeOffset.Now;
            var iat = now.ToUnixTimeSeconds();
            var payload = JsonConvert.SerializeObject(new JwtPayload()
            {
                iss = "IoT Gateway",
                iat = now.ToUnixTimeSeconds(),
                exp = now.AddMinutes(expiryInMinutes).ToUnixTimeSeconds(),
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
            if (String.IsNullOrEmpty(accessToken))
            {
                return false;
            }
            return accessToken == "DnR8TdVOO0eu8J9H9BsS2g==";
        }

        private static bool IsValidExpiry(string accessToken)
        {
            throw new NotImplementedException();
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
