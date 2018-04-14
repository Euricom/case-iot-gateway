using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common;
using Euricom.IoT.Security;
using System;
using Euricom.IoT.DataLayer.Interfaces;

namespace Euricom.IoT.Api.Managers
{
    public class SecurityManager : ISecurityManager
    {
        private readonly IUserRepository _userRepository;
        private static int _loginExpires = 60;
        private static int _wifiExpires = 1;

        public SecurityManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public string Login(string username, string password)
        {
            var user = _userRepository.Get(username);
            if (user == null || user.Check(password) == false)
            {
                throw new Exception("Invalid login");
            }

            var jwt = JwtSecurity.GenerateJwt(username, _loginExpires);
            return jwt;
        }

        public string LoginWithPuk(string puk)
        {
            if (puk != Constants.PUK)
                throw new Exception("PUK code invalid!");

            var jwt = JwtSecurity.GenerateJwt(puk, _loginExpires);
            return jwt;
        }

        public string RequestCommandToken(string accessToken)
        {
            // Verify if access token is valid
            var isValid = JwtSecurity.VerifyAccessTokenJwt(accessToken);
            if (isValid)
            {
                var user = "user";

                // Generate a jwt with a secret
                return JwtSecurity.GenerateJwt(user, _wifiExpires);
            }

            throw new UnauthorizedAccessException();
        }
        public bool ValidateToken(string jwt)
        {
            return JwtSecurity.VerifyJwt(jwt);
        }

        public void ChangePassword(string username, string old, string @new)
        {
            var user = _userRepository.Get(username);

            if (user.Check(old) == false)
            {
                throw new InvalidOperationException("Wrong password!");
            }

            user.UpdatePassword(@new);

            _userRepository.Update(user);
        }
    }
}
