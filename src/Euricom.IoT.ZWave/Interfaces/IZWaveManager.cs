using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euricom.IoT.ZWave.Interfaces
{
    public interface IZWaveManager
    {
        uint HomeId { get; }
        string CurrentStatus { get; }

        Task Initialize();

        bool TestConnection(byte nodeId);
        bool GetValue(byte nodeId, byte commandId);
        void SetValue(byte nodeId, byte commandId, bool value);
        List<Node> GetNodes();
    }
}