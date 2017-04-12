using Euricom.IoT.Models.Messages;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IGatewayManager
    {
        Task<bool> HandleMessage(GatewayMessage message);
    }
}
