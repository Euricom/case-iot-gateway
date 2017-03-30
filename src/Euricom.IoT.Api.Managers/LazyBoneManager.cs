using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.DataLayer;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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

        public Common.LazyBone Add(Common.LazyBone lazyBone)
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
                        break;
                    case "off":
                        await _lazyBone.Switch(false);
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
