using JoseRT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Euricom.IoT.Security
{
    public static class JwtSecurity
    {
        private static readonly byte[] SecretKey = { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };

        public static string GenerateJwt(string username, int expiryInMinutes, List<string> roles)
        {
            var now = DateTimeOffset.Now;
            var payload = JsonConvert.SerializeObject(new JwtPayload
            {
                Iss = "IoT Gateway",
                Iat = now.ToUnixTimeSeconds(),
                Exp = now.AddMinutes(expiryInMinutes).ToUnixTimeSeconds(),
                Sub = username,
                Roles = roles

            });

            return Jwt.Encode(payload, JwsAlgorithms.HS256, SecretKey);
        }

        public static JwtPayload DecodeJwt(string jwt)
        {
            string decoded = Jwt.Decode(jwt, SecretKey);
            return JsonConvert.DeserializeObject<JwtPayload>(decoded);
        }

        public static bool VerifyJwt(string jwt, out JwtPayload payload)
        {
            try
            {
                payload = DecodeJwt(jwt);

                // Check if token is not yet expired
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(payload.Exp).DateTime;
                if (expirationDate <= DateTime.UtcNow)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                payload = null;
                return false;
            }
        }
        
        public class JwtPayload
        {
            [JsonProperty("iss")]
            public string Iss { get; set; }
            [JsonProperty("iat")]
            public long Iat { get; set; }
            [JsonProperty("exp")]
            public long Exp { get; set; }
            [JsonProperty("sub")]
            public string Sub { get; set; }
            [JsonProperty("roles")]
            public List<string> Roles { get; set; }
        }
    }
}
