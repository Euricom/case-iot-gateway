using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.DataLayer;
using Euricom.IoT.LazyBone;
using Euricom.IoT.Logging;
using Euricom.IoT.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class SecurityManager : ISecurityManager
    {
        public SecurityManager()
        {
        }

        public string RequestCommandToken(string accessToken)
        {
            // TODO verify access token
            //var decodedJwt = JwtSecurity.DecodeJwt(accessToken);
            //if (decodedJwt.IsValid())
            {
                return JwtSecurity.GenerateJwt();
            }
            throw new UnauthorizedAccessException();
        }
    }
}
