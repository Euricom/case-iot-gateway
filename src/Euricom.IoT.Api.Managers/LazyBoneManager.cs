using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.DataLayer;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Euricom.IoT.Common;
using System.Collections.Generic;
using System.Linq;

namespace Euricom.IoT.Api.Managers
{
    public class LazyBoneManager : ILazyBoneManager
    {
        private IAzureDeviceManager _azureDeviceManager;
        private readonly LazyBone.LazyBone _lazyBone;

        public LazyBoneManager()
        {
            _azureDeviceManager = new AzureDeviceManager.AzureDeviceManager();
            _lazyBone = new LazyBone.LazyBone();
        }

        public Task<IEnumerable<Common.LazyBone>> GetAll()
        {
            var cameras = Database.Instance.GetLazyBones();
            return Task.FromResult(cameras.AsEnumerable());
        }

        public Task<Common.LazyBone> Get(string deviceId)
        {
            var json = Database.Instance.GetValue(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Common.LazyBone>(json));
        }

        public async Task<Common.LazyBone> Add(Common.LazyBone lazyBone)
        {
            //Add device to Azure Device IoT
            var deviceId = _azureDeviceManager.AddDeviceAsync(lazyBone.Name).Result;
            lazyBone.DeviceId = deviceId;

            //Convert to json
            var json = JsonConvert.SerializeObject(lazyBone);

            //Save to database
            Database.Instance.SetValue("LazyBones", lazyBone.DeviceId, json);

            return lazyBone;
        }

        public async Task<Common.LazyBone> Edit(Common.LazyBone lazyBone)
        {
            if (lazyBone == null)
            {
                throw new ArgumentNullException("lazyBone");
            }
            else if (String.IsNullOrEmpty(lazyBone.DeviceId))
            {
                throw new ArgumentException("lazyBone.DeviceId");
            }

            var json = JsonConvert.SerializeObject(lazyBone);

            Database.Instance.SetValue(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES, lazyBone.DeviceId, json);

            return await Get(lazyBone.DeviceId);
        }

        public async Task<bool> GetCurrentState(string deviceId)
        {
            try
            {
                var lazybone = DataLayer.Database.Instance.GetLazyBoneConfig(deviceId);
                var switchedOn = await _lazyBone.GetCurrentState(deviceId);
                return switchedOn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Remove(string deviceId)
        {
            try
            {
                // Remove device from Azure
                await _azureDeviceManager.RemoveDeviceAsync(deviceId);

                // Remove device from  database
                Database.Instance.RemoveDevice(deviceId);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Switch(string deviceId, string state)
        {
            //var config = DataLayer.Database.Instance.GetLazyBoneConfig(deviceId);

            if (string.IsNullOrEmpty(state))
            {
                throw new Exception("param state was null or empty");
            }
            else if (state != "on" && state != "off")
            {
                throw new Exception($"UNKNOWN parameter: { state}. Please use 'on' or 'off'");
            }

            try
            {
                switch (state)
                {
                    case "on":
                        await _lazyBone.Switch(true);
                        Debug.WriteLine("switched lazybone to ON");
                        break;
                    case "off":
                        await _lazyBone.Switch(false);
                        Debug.WriteLine("switched lazybone to OFF");
                        break;
                    default:
                        throw new InvalidOperationException($"unknown operation for LazyBone, state: {state}");
                }

                var notification = new LazyBoneNotification
                {
                    DeviceKey = deviceId,
                    State = state == "open" ? true : false,
                    Timestamp = DateTimeHelpers.Timestamp(),
                };

            }
            catch (Exception)
            {
                throw;
            }
        }

        Task<Common.LazyBone> ILazyBoneManager.Add(Common.LazyBone danaLock)
        {
            throw new NotImplementedException();
        }
    }
}
