using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer;
using Euricom.IoT.LazyBone;
using Euricom.IoT.Logging;
using Euricom.IoT.Messaging;
using Euricom.IoT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Managers
{
    public class LazyBoneManager : ILazyBoneManager
    {
        private readonly Database _database;

        public LazyBoneManager(Database database)
        {
            _database = database;
        }

        public Task<IEnumerable<Euricom.IoT.Models.LazyBone>> GetAll()
        {
            var lazyBones = _database.GetLazyBones();
            return Task.FromResult(lazyBones.AsEnumerable());
        }

        public Task<Euricom.IoT.Models.LazyBone> GetByDeviceId(string deviceId)
        {
            var json = _database.GetValue(Constants.DBREEZE_TABLE_LAZYBONES, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Euricom.IoT.Models.LazyBone>(json));
        }

        public string GetDeviceId(string deviceName)
        {
            var device = _database.GetCameras().FirstOrDefault(x => x.Name == deviceName);
            if (device == null)
            {
                throw new Exception($"Could not find deviceName: {deviceName}");
            }
            return device.DeviceId;
        }

        public Task<Euricom.IoT.Models.LazyBone> GetByDeviceName(string deviceName)
        {
            var deviceId = GetDeviceId(deviceName);
            var json = _database.GetValue(Constants.DBREEZE_TABLE_LAZYBONES, deviceId);
            return Task.FromResult(JsonConvert.DeserializeObject<Euricom.IoT.Models.LazyBone>(json));
        }

        public async Task<Euricom.IoT.Models.LazyBone> Add(Euricom.IoT.Models.LazyBone lazyBone)
        {
            // Generate Device Id
            lazyBone.DeviceId = Guid.NewGuid().ToString();

            //Convert to json
            var json = JsonConvert.SerializeObject(lazyBone);

            //Save to database
            _database.SetValue(Constants.DBREEZE_TABLE_LAZYBONES, lazyBone.DeviceId, json);

            return lazyBone;
        }

        public async Task<Euricom.IoT.Models.LazyBone> Edit(Euricom.IoT.Models.LazyBone lazyBone)
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

            _database.SetValue(Constants.DBREEZE_TABLE_LAZYBONES, lazyBone.DeviceId, json);

            return await GetByDeviceId(lazyBone.DeviceId);
        }

        public async Task Remove(string deviceName)
        {
            // Remove device from  database
            var deviceId = GetDeviceId(deviceName);
            _database.RemoveDevice(deviceId);
        }

        public async Task<bool> TestConnection(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            var config = _database.GetLazyBoneConfig(deviceId);
            return await LazyBoneConnectionManager.Instance.TestConnection(deviceId, config);
        }

        public async Task<bool> GetCurrentStateSwitch(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            try
            {
                var config = _database.GetLazyBoneConfig(deviceId);
                if (!config.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
                }
                if (config.IsDimmer)
                    throw new InvalidOperationException("device is not a switch, but a dimmer");

                return await LazyBoneConnectionManager.Instance.GetCurrentStateSwitch(deviceId, config);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public async Task<LazyBoneDimmerState> GetCurrentStateDimmer(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            try
            {
                var config = _database.GetLazyBoneConfig(deviceId);
                if (!config.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not checking device state because device is not enabled");
                    throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
                }
                if (!config.IsDimmer)
                    throw new InvalidOperationException("device is a switch, not a dimmer");

                return await LazyBoneConnectionManager.Instance.GetCurrentStateDimmer(deviceId, config);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }

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
                var settings = _database.GetConfigSettings();
                var config = _database.GetLazyBoneConfig(deviceId);

                if (!config.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not switching because device is not enabled");
                    throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
                }

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

                // Log command
                Logger.Instance.LogInformationWithDeviceContext(deviceId, $"Changed state: {state}");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public async Task SetLightValue(string deviceId, short? value)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }
            else if (value < 0 || value > 255)
            {
                throw new ArgumentOutOfRangeException("value must be between 0 and 255 inclusive");
            }

            try
            {
                var settings = _database.GetConfigSettings();
                var config = _database.GetLazyBoneConfig(deviceId);

                if (!config.Enabled)
                {
                    Logger.Instance.LogWarningWithDeviceContext(deviceId, "Not setting light value because device is not enabled");
                    throw new InvalidOperationException($"Device: {config.Name} {deviceId} is not enabled");
                }

                await LazyBoneConnectionManager.Instance.SetLightValue(deviceId, config, value);

                // Log command
                Logger.Instance.LogInformationWithDeviceContext(deviceId, $"Changed light value: {value}");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public async Task TestChangeLightIntensity(string deviceId)
        {
            // Set to 50
            await SetLightValue(deviceId, 50);
            await Task.Delay(1500);

            // Set to 100
            await SetLightValue(deviceId, 100);
            await Task.Delay(1500);

            // Set to 255
            await SetLightValue(deviceId, 255);
        }
    }
}
