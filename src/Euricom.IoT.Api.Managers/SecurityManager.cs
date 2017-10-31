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

        public string LoginWithPUK(string puk)
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
                // TODO get user from accesstoken
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

        public void LostPassword()
        {
            // Generate a GUID
            var resetGuid = Guid.NewGuid();

            // Create a body template and include a link


            // Include GUID in mail link


            // Send mail
            //_mailer.SendLostPasswordMail("wim.vandenrul@euri.com", )
        }

        public void ResetPassword(string resetGuid)
        {
            // 
            // Add to d
            // _database.VerifyResetPasswordGuid(resetGuid);

            // Generate a new password
            // var password = RandomPasswordGenerator.CreateRandomPassword(10);

            // Generate a verification id


            // Send mail

        }
    }
}
