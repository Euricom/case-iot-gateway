using DBreeze;
using Euricom.IoT.Common;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Euricom.IoT.DataLayer
{
    public class Database : IDisposable, IDatabase
    {
        private readonly DBreeze.DBreezeEngine _engine;

        public Database(DBreezeEngine engine)
        {
            _engine = engine;
        }
        
        public bool ExistsUser(string username)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var row = tran.Select<string, string>(Constants.DBREEZE_TABLE_USERS, username);
                    return row.Exists;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public void AddUser(string username, string password)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var row = tran.Select<string, string>(Constants.DBREEZE_TABLE_USERS, username);
                    if (row.Exists)
                        throw new InvalidOperationException($"username {username} already exists in the database");

                    tran.Insert<string, string>(Constants.DBREEZE_TABLE_USERS, username, password);
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public bool CheckUser(string username, string password)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var row = tran.Select<string, string>(Constants.DBREEZE_TABLE_USERS, username);
                    if (row.Exists && row.Value == password)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public bool EditPassword(string username, string password)
        {
            try
            {
                bool wasUpdated = false;
                byte[] refToInsertedValue = new List<byte>().ToArray();

                using (var tran = _engine.GetTransaction())
                {
                    var row = tran.Select<string, string>(Constants.DBREEZE_TABLE_USERS, username);
                    if (row.Exists && row.Value == password)
                    {
                        tran.Insert<string, string>(Constants.DBREEZE_TABLE_USERS, username, password, out refToInsertedValue, out wasUpdated);
                    }
                    return wasUpdated;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public void RemoveUser(string username)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var row = tran.Select<string, string>(Constants.DBREEZE_TABLE_USERS, username);
                    if (!row.Exists)
                        throw new InvalidOperationException($"cannot remove username {username}, because it doesn't exist in the database");

                    if (row.Exists)
                        tran.RemoveKey(Constants.DBREEZE_TABLE_USERS, username);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }


        public DanaLock GetDanaLockConfig(string deviceId)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var json = tran.Select<string, string>(Constants.DBREEZE_TABLE_DANALOCKS, deviceId).Value;
                    return JsonConvert.DeserializeObject<DanaLock>(json);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public WallMountSwitch GetWallMountConfig(string deviceId)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var json = tran.Select<string, string>(Constants.DBREEZE_TABLE_WALLMOUNTS, deviceId).Value;
                    return JsonConvert.DeserializeObject<WallMountSwitch>(json);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public Settings GetConfigSettings()
        {
            var settings = new Settings();
            settings.HistoryLog = GetValueAsInt(Constants.DBREEZE_TABLE_SETTINGS, "HistoryLog");

            if (String.IsNullOrEmpty(GetValue(Constants.DBREEZE_TABLE_SETTINGS, "LogLevel")))
                settings.LogLevel = LogLevel.Information;
            else
                settings.LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), GetValue(Constants.DBREEZE_TABLE_SETTINGS, "LogLevel"));

            settings.GatewayDeviceKey = GetValue(Constants.DBREEZE_TABLE_SETTINGS, "GatewayDeviceKey");
            settings.AzureIotHubUri = GetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUri");
            settings.AzureIotHubUriConnectionString = GetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUriConnectionString");
            settings.AzureAccountName = GetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureAccountName");
            settings.AzureStorageAccessKey = GetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureStorageAccessKey");
            settings.DropboxAccessToken = GetValue(Constants.DBREEZE_TABLE_SETTINGS, "DropboxAccessToken");
            return settings;
        }

        private int GetValueAsInt(string table, string key)
        {
            string v = GetValue(table, key);
            if (String.IsNullOrEmpty(v))
                return 31;
            return Int32.Parse(v);
        }

        public void SaveConfigSettings(Settings settings)
        {
            if (settings != null)
            {
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "HistoryLog", settings.HistoryLog.ToString());
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "LogLevel", settings.LogLevel.ToString());
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "GatewayDeviceKey", settings.GatewayDeviceKey);
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUri", settings.AzureIotHubUri);
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUriConnectionString", settings.AzureIotHubUriConnectionString);
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureAccountName", settings.AzureAccountName);
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureStorageAccessKey", settings.AzureStorageAccessKey);
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "DropboxAccessToken", settings.DropboxAccessToken);
            }

            if (settings != null && !String.IsNullOrEmpty(settings.Password))
            {
                Logger.Instance.LogWarningWithContext(this.GetType(), "Password changed");
                SetValue(Constants.DBREEZE_TABLE_USERS, "admin", settings.Password);
            }
        }

        public LazyBone GetLazyBoneConfig(string deviceId)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var json = tran.Select<string, string>(Constants.DBREEZE_TABLE_LAZYBONES, deviceId).Value;
                    return JsonConvert.DeserializeObject<LazyBone>(json);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public Device FindDevice(string deviceId)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_CAMERAS))
                    {
                        if (row.Key == deviceId)
                        {
                            var camera = JsonConvert.DeserializeObject<Camera>(row.Value);
                            return camera;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_DANALOCKS))
                    {
                        if (row.Key == deviceId)
                        {
                            var danalock = JsonConvert.DeserializeObject<DanaLock>(row.Value);
                            return danalock;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_LAZYBONES))
                    {
                        if (row.Key == deviceId)
                        {
                            var lazybone = JsonConvert.DeserializeObject<LazyBone>(row.Value);
                            return lazybone;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public bool RemoveDevice(string deviceId)
        {
            try
            {
                bool removed = false;
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_CAMERAS))
                    {
                        if (row.Key == deviceId)
                        {
                            tran.RemoveKey(Constants.DBREEZE_TABLE_CAMERAS, row.Key);
                            removed = true;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_DANALOCKS))
                    {
                        if (row.Key == deviceId)
                        {
                            tran.RemoveKey(Constants.DBREEZE_TABLE_DANALOCKS, row.Key);
                            removed = true;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_LAZYBONES))
                    {
                        if (row.Key == deviceId)
                        {
                            tran.RemoveKey(Constants.DBREEZE_TABLE_LAZYBONES, row.Key);
                            removed = true;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_WALLMOUNTS))
                    {
                        if (row.Key == deviceId)
                        {
                            tran.RemoveKey(Constants.DBREEZE_TABLE_WALLMOUNTS, row.Key);
                            removed = true;
                        }
                    }

                    // Commit changes
                    tran.Commit();
                }

                if (!removed)
                {
                    throw new Exception("Remove failed because key was not found in any table");
                }

                return removed;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public Camera GetCameraConfig(string deviceId)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var json = tran.Select<string, string>(Constants.DBREEZE_TABLE_CAMERAS, deviceId).Value;
                    var cameraConfig = JsonConvert.DeserializeObject<Camera>(json);
                    return cameraConfig;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public Hardware GetHardware()
        {
            try
            {
                var hardware = new Hardware();
                hardware.Cameras = GetCameras();
                hardware.LazyBones = GetLazyBones();
                hardware.DanaLocks = GetDanaLocks();
                hardware.WallMountSwitches = GetWallMountSwitches();
                return hardware;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public List<Camera> GetCameras()
        {
            try
            {
                List<Camera> cameras = new List<Camera>();
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_CAMERAS))
                    {
                        var deviceGuid = row.Key;
                        var deviceConfig = JsonConvert.DeserializeObject<Camera>(row.Value);
                        cameras.Add(deviceConfig);
                    }
                }
                return cameras;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public List<DanaLock> GetDanaLocks()
        {
            try
            {
                List<DanaLock> danaLocks = new List<DanaLock>();
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_DANALOCKS))
                    {
                        var deviceGuid = row.Key;
                        var deviceConfig = JsonConvert.DeserializeObject<DanaLock>(row.Value);
                        danaLocks.Add(deviceConfig);
                    }
                }
                return danaLocks;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public List<LazyBone> GetLazyBones()
        {
            try
            {
                List<LazyBone> lazyBones = new List<LazyBone>();
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_LAZYBONES))
                    {
                        var deviceGuid = row.Key;
                        var deviceConfig = JsonConvert.DeserializeObject<LazyBone>(row.Value);
                        lazyBones.Add(deviceConfig);
                    }
                    tran.Commit();
                }
                return lazyBones;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public List<WallMountSwitch> GetWallMountSwitches()
        {
            try
            {
                List<WallMountSwitch> wallmounts = new List<WallMountSwitch>();
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>(Constants.DBREEZE_TABLE_WALLMOUNTS))
                    {
                        var deviceGuid = row.Key;
                        var deviceConfig = JsonConvert.DeserializeObject<WallMountSwitch>(row.Value);
                        wallmounts.Add(deviceConfig);
                    }
                }
                return wallmounts;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw;
            }
        }

        public string GetValue(string table, string key)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    return tran.Select<string, string>(table, key).Value;
                }
            }
            catch (Exception ex)
            {
                //Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                //var exception = new Exception($"Could not get value for table: {table}, key: {key}, exception: " + ex);
                //Logger.Instance.LogErrorWithContext(this.GetType(), exception);
                //throw exception;

                Logger.Instance.LogWarningWithContext(this.GetType(), ex.ToString());
                return null;
            }
        }

        public void SetValue(string table, string key, string value)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    tran.Insert<string, string>(table, key, value);
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not set value for table: {table}, key: {key}, exception: " + ex);
            }
        }

        private void InitializeSettings()
        {
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "HistoryLog", 365.ToString());
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUri", Secrets.AZURE_IOT_HUB_URI);
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUriConnectionString", Secrets.AZURE_IOT_HUB_CONNECTIONSTRING);
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureAccountName", Secrets.AZURE_ACCOUNT_NAME);
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureStorageAccessKey", Secrets.AZURE_STORAGE_ACCESS_KEY);
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "DropboxAccessToken", Secrets.DROPBOX_ACCESS_TOKEN);
        }

        public void Dispose()
        {
            if (_engine != null)
            {
                _engine.Dispose();
            }
        }
    }
}
