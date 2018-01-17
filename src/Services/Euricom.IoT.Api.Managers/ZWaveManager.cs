using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Interfaces;

namespace Euricom.IoT.Api.Managers
{
    public class ZWaveManager: IZWaveManager
    {
        private readonly IZWaveController _izWaveController;

        public ZWaveManager(IZWaveController izWaveController)
        {
            _izWaveController = izWaveController;
        }

        public async Task Initialize()
        {
            await _izWaveController.Initialize();
        }

        public async Task SoftReset()
        {
            await _izWaveController.SoftReset();
        }
        
        public List<NodeDto> GetNodes()
        {
            var nodes = _izWaveController.GetNodes();

            return Mapper.Map<List<NodeDto>>(nodes);
        }

        public void AddNode(bool secure)
        {
            _izWaveController.AddNode(secure);
        }

        public void RemoveNode()
        {
            _izWaveController.RemoveNode();
        }

        public string GetStatus()
        {
            return _izWaveController.GetStatus();
        }
    }
}
