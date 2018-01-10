using System.Collections.Generic;
using System.Threading.Tasks;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Managers.Interfaces
{
    public interface IZWaveManager
    {
        Task Initialize();
        Task SoftReset();
        List<NodeDto> GetNodes();
        void AddNode(bool secure);
        void RemoveNode();
        string GetStatus();
    }
}