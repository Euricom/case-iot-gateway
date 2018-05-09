using System;
using System.Collections.Generic;
using System.Linq;
using Euricom.IoT.Common.Exceptions;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Devices.ZWave;

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
            var device = _database.ZWaveDevices.FirstOrDefault(d => d.NodeId == nodeId);

            if (device == null)
            {
                throw new NotFoundException(nodeId.ToString());
            }

            return device;
        }

        private ZWaveDevice Get(string deviceId)
        {
            var device = _database.ZWaveDevices.FirstOrDefault(d => d.DeviceId == deviceId);

            if (device == null)
            {
                throw new NotFoundException(deviceId);
            }

            return device;
        }

        public void UpdateZWaveDevice(ZWaveDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            var d = Get(device.DeviceId);

            _database.Entry(d).CurrentValues.SetValues(device);
            _database.SaveChanges();
        }
    }
}