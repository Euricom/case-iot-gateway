using Euricom.IoT.Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IGatewayManager
    {
        Task<bool> HandleMessage(GatewayMessage message);
    }
}
