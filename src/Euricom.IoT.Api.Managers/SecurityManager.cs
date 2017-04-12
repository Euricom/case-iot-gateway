using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Security;
using System;

namespace Euricom.IoT.Api.Managers
{
    public class SecurityManager : ISecurityManager
    {
        public SecurityManager()
        {
            if (!DataLayer.Database.Instance.ExistsUser("admin"))
            {
                DataLayer.Database.Instance.AddUser("admin", "admin");
            }
        }

        public string Login(string username, string password)
        {
            var valid = DataLayer.Database.Instance.CheckUser(username, password);
            if (!valid)
                throw new Exception("Invalid login");

            var jwt = JwtSecurity.GenerateJwt(username);
            return jwt;
        }

        public string RequestCommandToken(string accessToken)
        {
            var isValid = JwtSecurity.VerifyAccessTokenJwt(accessToken);
            if (isValid)
            {
                return JwtSecurity.GenerateJwt("_IOT_GATEWAY_");
            }
            throw new UnauthorizedAccessException();
        }
        public bool ValidateToken(string jwt)
        {
            return JwtSecurity.VerifyJwt(jwt);
        }
    }
}
