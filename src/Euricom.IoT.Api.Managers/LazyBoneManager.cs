using AzureDeviceManager;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.DataLayer;
using Newtonsoft.Json;
using System;

namespace Euricom.IoT.Api.Managers
{
    public class LazyBoneManager : ILazyBoneManager
    {
        private DeviceManager _azureDeviceManager;
        private readonly LazyBone.LazyBone _lazyBone;

        public LazyBoneManager()
        {
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

        public void Switch(string device, string state)
        {
            var config = DataLayer.Database.Instance.GetLazyBoneConfig(device);

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
                        _lazyBone.Switch(true);
                        break;
                    case "off":
                        _lazyBone.Switch(false);
                        break;
                    default:
                        throw new InvalidOperationException($"unknown operation for LazyBone, state: {state}");
                }

                var notification = new LazyBoneNotification
                {
                    DeviceKey = config.DeviceId,
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
