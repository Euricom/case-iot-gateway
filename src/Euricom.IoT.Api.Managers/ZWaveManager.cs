using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Managers
{
    public class ZWaveManager: IZWaveManager
    {
        private readonly ZWave.Interfaces.IZWaveManager _zWaveManager;

        public ZWaveManager(ZWave.Interfaces.IZWaveManager zWaveManager)
        {
            _zWaveManager = zWaveManager;
        }

        public async Task Initialize()
        {
            await _zWaveManager.Initialize();
        }

        public async Task SoftReset()
        {
            await _zWaveManager.SoftReset();
        }
        
        public List<NodeDto> GetNodes()
        {
            var nodes = _zWaveManager.GetNodes();

            return Mapper.Map<List<NodeDto>>(nodes);
        }

        public void AddNode(bool secure)
        {
            _zWaveManager.AddNode(secure);
        }

        public void RemoveNode()
        {
            _zWaveManager.RemoveNode();
        }
    }
}
