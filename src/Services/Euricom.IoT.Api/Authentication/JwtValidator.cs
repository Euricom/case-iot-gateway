using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Restup.Webserver.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Authentication
{
    public class JwtValidator : ICredentialValidator
    {
        private readonly ISecurityManager _securityManager;

        public JwtValidator(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        public bool Authenticate(string username, string password)
        {
            try
            {
                _securityManager.Login(username, password);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
