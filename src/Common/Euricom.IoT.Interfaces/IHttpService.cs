using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IHttpService
    {
        Task<bool> TestConnection(string address, string server);
    }
}
