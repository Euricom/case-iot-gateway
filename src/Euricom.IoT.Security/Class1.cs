using JoseRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Security
{
    public class JwtGenerator
    {
        public JwtGenerator()
        {
            
        }

        public string GenerateJwt()
        {
            var secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };
            string payload = @"{""hello"" : ""world""}";
            string jwt = JoseRT.Jwt.Encode(payload, JwsAlgorithms.HS256, secretKey);
            return jwt;
        }

        public string DecodeJwt(string jwt)
        {
            byte[] secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };
            string decoded = JoseRT.Jwt.Decode(jwt, secretKey);
            return decoded;
        }
    }
}
