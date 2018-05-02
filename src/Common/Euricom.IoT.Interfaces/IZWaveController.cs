using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.Interfaces
{
    public interface IZWaveController
    {
        Task SoftReset();
        Task Initialize(IZWaveDeviceNotifier notifier, string networkKey);

        bool TestConnection(byte nodeId);
        bool GetValue(byte nodeId, byte commandId);
        void SetValue(byte nodeId, byte commandId, bool value);
        List<INode> GetNodes();
        void RemoveNode();
        void AddNode(bool secure);
        string GetStatus();
    }
}