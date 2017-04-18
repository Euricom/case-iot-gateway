using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OpenZWave;

namespace Euricom.IoT.ZWave
{
    public interface IZWaveManager
    {
        string CurrentStatus { get; }
        uint HomeId { get; }
        ZWaveManager.NodeQueryStatus QueryStatus { get; }
        ObservableCollection<SerialPortInfo> SerialPorts { get; }
        ZWManager ZWManager { get; }

        Task Initialize();
        bool TestConnection(byte nodeId);
    }
}