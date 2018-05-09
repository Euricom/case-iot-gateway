using System;
using System.Collections.Generic;
using System.Linq;
using Euricom.IoT.Common.Exceptions;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Models;

namespace Euricom.IoT.DataLayer
{
    public class DeviceRepository<TDevice> : IDeviceRepository<TDevice> where TDevice : Device
    {
        private readonly IotDbContext _database;

        public DeviceRepository(IotDbContext database)
        {
            _database = database;
        }

        public void Add(TDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            try
            {
                _database.Set<TDevice>().Add(device);
                _database.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.HandleAlreadyExistsException();
                throw;
            }
        }

        public void Remove(string id)
        {
            var device = _database
                .Set<TDevice>()
                .FirstOrDefault(d => d.DeviceId == id);

            if (device != null)
            {
                _database.Set<TDevice>().Remove(device);
                _database.SaveChanges();
            }
        }

        public TDevice Get(string id)
        {
            var device = _database
                .Set<TDevice>()
                .FirstOrDefault(d => d.DeviceId == id);

            if (device == null)
            {
                throw new NotFoundException(id);
            }

            return device;
        }

        public IEnumerable<TDevice> Get()
        {
            return _database.Set<TDevice>().ToList();
        }

        public void Update(TDevice device)
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