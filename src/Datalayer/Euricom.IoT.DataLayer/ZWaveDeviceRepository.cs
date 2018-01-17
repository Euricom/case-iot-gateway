using System;
using System.Linq;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.ZWave;
using Euricom.IoT.Logging;

namespace Euricom.IoT.DataLayer
{
    public class ZWaveDeviceRepository : IZWaveDeviceRepository
    {
        private readonly IotDbContext _database;

        public ZWaveDeviceRepository(IotDbContext database)
        {
            _database = database;
        }

        public ZWaveDevice GetZWaveDevice(byte nodeId)
        {
            try
            {
                return _database.ZWaveDevices.FirstOrDefault(d => d.NodeId == nodeId);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
                throw;
            }
        }
    }
}