using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common;
using Euricom.IoT.Mailing;
using Euricom.IoT.Security;
using System;

namespace Euricom.IoT.Api.Managers
{
    public class SecurityManager : ISecurityManager
    {
        private IMailer _mailer;
        private static int _loginExpires = 60;
        private static int _wifiExpires = 1;

        public SecurityManager(Mailer mailer)
        {
            _mailer = mailer;

            if (!DataLayer.Database.Instance.ExistsUser("admin"))
            {
                DataLayer.Database.Instance.AddUser("admin", "secret_password");
            }
        }

        public string Login(string username, string password)
        {
            var valid = DataLayer.Database.Instance.CheckUser(username, password);
            if (!valid)
                throw new Exception("Invalid login");

            var jwt = JwtSecurity.GenerateJwt(username, _loginExpires);
            return jwt;
        }

        public string LoginWithPUK(string PUK)
        {
            if (PUK != Constants.PUK)
                throw new Exception("PUK code invalid!");

            var jwt = JwtSecurity.GenerateJwt(PUK, _loginExpires);
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
            // DataLayer.Database.Instance.VerifyResetPasswordGuid(resetGuid);

            // Generate a new password
            // var password = RandomPasswordGenerator.CreateRandomPassword(10);

            // Generate a verification id


            // Send mail

        }
    }
}
