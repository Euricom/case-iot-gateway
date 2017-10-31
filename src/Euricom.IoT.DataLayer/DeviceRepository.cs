using System;
using System.Collections.Generic;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;

namespace Euricom.IoT.DataLayer
{
    public class DeviceRepository<TDevice>: IDeviceRepository<TDevice> where TDevice : Device
    {
        private readonly IDbBreezeDatabase _database;

        public DeviceRepository(IDbBreezeDatabase database)
        {
            _database = database;
        }

        public void Add(TDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            var table = GetTableName();

            try
            {
                _database.SetValue(table, device.DeviceId, device);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw new Exception($"Could not set value for table: {table}, key: {device.DeviceId}, exception: " + ex);
            }
        }

        public void Remove(string id)
        {
            var table = GetTableName();
            try
            {
                using (var tran = _database.GetTransaction())
                {
                    tran.RemoveKey(table, id);
                    tran.Commit();
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
            var table = GetTableName();
            try
            {
                return _database.GetValue<TDevice>(table, id);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(id, ex);
                throw;
            }
        }

        public IEnumerable<TDevice> Get()
        {
            var table = GetTableName();
            try
            {
                return _database.GetValues<TDevice>(table);
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

            var table = GetTableName();
            try
            {
                _database.SetValue(table, device.DeviceId, device);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(GetType(), ex);
                throw;
            }
        }

        private string GetTableName()
        {
            switch (Enum.Parse(typeof(HardwareType), typeof(TDevice).Name))
            {
                case HardwareType.Camera:
                    return Constants.DBREEZE_TABLE_CAMERAS;
                case HardwareType.DanaLock:
                    return Constants.DBREEZE_TABLE_DANALOCKS;
                case HardwareType.LazyBoneSwitch:
                case HardwareType.LazyBoneDimmer:
                    return Constants.DBREEZE_TABLE_LAZYBONES;
                case HardwareType.WallMountSwitch:
                    return Constants.DBREEZE_TABLE_WALLMOUNTS;
                default:
                    throw new ArgumentOutOfRangeException(nameof(TDevice), typeof(TDevice).Name, null);
            }
        }
    }
}