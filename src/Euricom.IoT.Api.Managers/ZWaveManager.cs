using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Euricom.IoT.Api.Models;
using Euricom.IoT.Devices.ZWave;
using Euricom.IoT.Devices.ZWave.Interfaces;
using IZWaveManager = Euricom.IoT.Api.Managers.Interfaces.IZWaveManager;

namespace Euricom.IoT.Api.Managers
{
    public class ZWaveDeviceNotifier : IZWaveDeviceNotifier
    {
        public async Task Notify(byte nodeId, byte key, byte value)
        {
            Debug.WriteLine($"NodeID: {nodeId}, Key: {key}, Value: {value}");
        }
    }

    public class ZWaveManager: IZWaveManager
    {
        private readonly Devices.ZWave.Interfaces.IZWaveManager _zWaveManager;

        public ZWaveManager(Devices.ZWave.Interfaces.IZWaveManager zWaveManager)
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

        public string GetStatus()
        {
            return _zWaveManager.GetStatus();
        }
    }
}
