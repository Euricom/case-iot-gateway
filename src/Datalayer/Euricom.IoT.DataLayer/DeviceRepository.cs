using System;
using System.Collections.Generic;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;

namespace Euricom.IoT.DataLayer
{
    public class DeviceRepository<TDevice>: IDeviceRepository<TDevice> where TDevice : Device
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
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not set value for device, key: {device.DeviceId}, exception: " + ex);
            }
        }

        public void Remove(string id)
        {
            try
            {
                var device = _database.Set<TDevice>().Find(id);

                if (device != null)
                {
                    _database.Set<TDevice>().Remove(device);
                    _database.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(id, ex);
                throw;
            }
        }

        public TDevice Get(string id)
        {
            try
            {
                return _database.Set<TDevice>().Find(id);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(id, ex);
                throw;
            }
        }

        public IEnumerable<TDevice> Get()
        {
            try
            {
                return _database.Set<TDevice>();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }

        public void Update(TDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }
            
            try
            {
                var d =_database.Set<TDevice>().Find(device.DeviceId);
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