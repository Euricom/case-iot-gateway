using System;
using System.Collections.Generic;
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

        public List<ZWaveDevice> GetDevices()
        {
            return _database.ZWaveDevices.ToList();
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

        public void UpdateZWaveDevice(ZWaveDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            try
            {
                var d = _database.ZWaveDevices.Find(device.DeviceId);
                _database.Entry(d).CurrentValues.SetValues(device);
                _database.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }
    }
}