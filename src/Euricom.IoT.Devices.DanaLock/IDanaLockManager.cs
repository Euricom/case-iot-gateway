using System.Threading.Tasks;

namespace Euricom.IoT.DanaLock
{
    public interface IDanaLockManager
    {
        bool TestConnection(byte nodeId);
        bool IsLocked(byte nodeId);
        void CloseLock(byte nodeId);
        void OpenLock(byte nodeId);
    }
}