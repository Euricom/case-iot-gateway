using System.Collections.Generic;
using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.ZWave.Interfaces;

namespace Euricom.IoT.Api.Managers
{
    public class ZWaveManager
    {
        private readonly IZWaveManager _zWaveManager;

        public ZWaveManager(IZWaveManager zWaveManager)
        {
            _zWaveManager = zWaveManager;
        }

        public void Initialize()
        {
            _zWaveManager.Initialize();
        }

        public List<NodeDto> GetNodes()
        {
            var nodes = _zWaveManager.GetNodes();

            return Mapper.Map<List<NodeDto>>(nodes);
        }
    }
}
