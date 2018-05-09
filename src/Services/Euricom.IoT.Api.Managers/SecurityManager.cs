using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common;
using Euricom.IoT.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using Euricom.IoT.Common.Exceptions;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;

namespace Euricom.IoT.Api.Managers
{
    public class SecurityManager : ISecurityManager
    {
        private readonly IUserRepository _userRepository;

        private const int LoginExpires = 1;
        private const int WifiExpires = 5;

        public SecurityManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public string Login(string username, string password)
        {
            User user;
            try
            {
                user = _userRepository.Get(username);

                if (user.Check(password) == false)
                {
                    throw new UnauthorizedException();
                }
            }
            catch (NotFoundException)
            {
                throw new UnauthorizedException();
            }

            return JwtSecurity.GenerateJwt(username, LoginExpires, user.Roles.Select(r => r.RoleName).ToList());
        }

        public string LoginWithPuk(string puk)
        {
            if (puk != Constants.PUK)
            {
                throw new UnauthorizedException();
            }

            return JwtSecurity.GenerateJwt(puk, LoginExpires, new List<string> { "Administrator", "User", "Manager" });
        }

        public string RequestCommandToken(string token)
        {
            User user;
            try
            {
                user = _userRepository.GetUserWithToken(token);
            }
            catch (NotFoundException)
            {
                Logger.Instance.Warning($"Unknown user with token {token} tried to request a command token.");
                throw new UnauthorizedException();
            }

            Logger.Instance.Debug($"User with token {token} requested command token.");
            // Generate a jwt with a secret
            return JwtSecurity.GenerateJwt(user.Username, WifiExpires, user.Roles.Select(r => r.RoleName).ToList());
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
