using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common;
using Euricom.IoT.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Logging;

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

            var jwt = JwtSecurity.GenerateJwt(username, _loginExpires, user.Roles.Select(r => r.RoleName).ToList());
            return jwt;
        }

        public string LoginWithPuk(string puk)
        {
            if (puk != Constants.PUK)
                throw new Exception("PUK code invalid!");

            var jwt = JwtSecurity.GenerateJwt(puk, _loginExpires, new List<string> { "Administrator", "User", "Manager" });
            return jwt;
        }

        public string RequestCommandToken(string token)
        {
            var user = _userRepository.GetUserWithToken(token);
            if (user != null)
            {
                Logger.Instance.LogDebugWithContext(GetType(), $"User with token {token} requested command token.");
                // Generate a jwt with a secret
                return JwtSecurity.GenerateJwt(user.Username, _wifiExpires, user.Roles.Select(r => r.RoleName).ToList());
            }

            Logger.Instance.LogWarningWithContext(GetType(), $"Unknown user with token {token} tried to request a command token.");
            throw new UnauthorizedAccessException();
        }
        public bool ValidateToken(string jwt, out JwtSecurity.JwtPayload payload)
        {
            return JwtSecurity.VerifyJwt(jwt, out payload);
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
