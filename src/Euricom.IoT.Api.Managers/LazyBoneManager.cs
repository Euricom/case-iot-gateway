using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.DataLayer;
using Euricom.IoT.LazyBone;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class LazyBoneManager : ILazyBoneManager
    {
        private IAzureDeviceManager _azureDeviceManager;


        public LazyBoneManager()
        {
            _azureDeviceManager = new AzureDeviceManager.AzureDeviceManager();
        }

        public Task<IEnumerable<Common.LazyBone>> GetAll()
        {
            var lazyBones = Database.Instance.GetLazyBones();
            return Task.FromResult(lazyBones.AsEnumerable());
        }

        public Task<Common.LazyBone> Get(string deviceId)
        {
            var json = Database.Instance.GetValue(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Common.LazyBone>(json));
        }

        public async Task<Common.LazyBone> Add(Common.LazyBone lazyBone)
        {
            //Add device to Azure Device IoT
            //var deviceId = await _azureDeviceManager.AddDeviceAsync(lazyBone.Name).Result;
            lazyBone.DeviceId = Guid.NewGuid().ToString();

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

        public async Task<bool> Remove(string deviceId)
        {
            try
            {
                // Remove device from Azure
                // await _azureDeviceManager.RemoveDeviceAsync(deviceId);

                // Remove device from  database
                Database.Instance.RemoveDevice(deviceId);

                //// Do not poll this device anymore
                //MonitoringSystem.Instance.RemoveMonitor(deviceId);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

 
    
        public async Task<string> TestConnection(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            var config = Database.Instance.GetLazyBoneConfig(deviceId);
            return await LazyBoneConnectionManager.Instance.TestConnection(deviceId, config);
        }

        public async Task<bool> GetCurrentState(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            var config = Database.Instance.GetLazyBoneConfig(deviceId);
            return await LazyBoneConnectionManager.Instance.GetCurrentState(deviceId, config);
        }

        public async Task Switch(string deviceId, string state)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }
            else if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException("state");
            }
            else if (state != "on" && state != "off")
            {
                throw new ArgumentException($"UNKNOWN parameter: { state}. Please use 'on' or 'off'");
            }

            try
            {
                var config = Database.Instance.GetLazyBoneConfig(deviceId);
                switch (state)
                {
                    case "on":
                        await LazyBoneConnectionManager.Instance.Switch(deviceId, config, true);
                        break;
                    case "off":
                        await LazyBoneConnectionManager.Instance.Switch(deviceId, config, false);
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
    }
}
