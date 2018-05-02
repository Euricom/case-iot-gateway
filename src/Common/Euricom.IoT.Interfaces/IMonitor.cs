using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IMonitor
    {
        void StartMonitoring();
        void StopMonitoring();
    }
}