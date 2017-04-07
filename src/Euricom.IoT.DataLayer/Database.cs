using DBreeze;
using Euricom.IoT.Common;
using Euricom.IoT.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace Euricom.IoT.DataLayer
{
    public class Database : IDisposable, IDatabase
    {
        private static readonly Database _instance = new Database();

        private static DBreeze.DBreezeEngine _engine;


        private Database()
        {
            InitDB();

            //TODO remove once settings page in angular works
            // InitializeSettings();
        }

        public static Database Instance
        {
            get
            {
                return _instance;
            }
        }

        public DanaLock GetDanaLockConfig(string deviceId)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var json = tran.Select<string, string>(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS, deviceId).Value;
                    return JsonConvert.DeserializeObject<DanaLock>(json);
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
            settings.HistoryLog = Int32.Parse(GetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "HistoryLog"));
            settings.AzureIotHubUri = GetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureIotHubUri");
            settings.AzureIotHubUriConnectionString = GetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureIotHubUriConnectionString");
            settings.AzureAccountName = GetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureAccountName");
            settings.AzureStorageAccessKey = GetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureStorageAccessKey");
            settings.DropboxAccessToken = GetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "DropboxAccessToken");
            return settings;
        }

        public void SaveConfigSettings(Common.Settings settings)
        {
            if (settings != null)
            {
                SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "HistoryLog", settings.HistoryLog.ToString());
                SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureIotHubUri", settings.AzureIotHubUri);
                SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureIotHubUriConnectionString", settings.AzureIotHubUriConnectionString);
                SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureAccountName", settings.AzureAccountName);
                SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureStorageAccessKey", settings.AzureStorageAccessKey);
                SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "DropboxAccessToken", settings.DropboxAccessToken);
            }
        }

        public LazyBone GetLazyBoneConfig(string deviceId)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var json = tran.Select<string, string>(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES, deviceId).Value;
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
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_CAMERAS))
                    {
                        if (row.Key == deviceId)
                        {
                            var camera = JsonConvert.DeserializeObject<Camera>(row.Value);
                            return camera;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS))
                    {
                        if (row.Key == deviceId)
                        {
                            var danalock = JsonConvert.DeserializeObject<DanaLock>(row.Value);
                            return danalock;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES))
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
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_CAMERAS))
                    {
                        if (row.Key == deviceId)
                        {
                            tran.RemoveKey(DatabaseTableNames.DBREEZE_TABLE_CAMERAS, row.Key);
                            removed = true;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS))
                    {
                        if (row.Key == deviceId)
                        {
                            tran.RemoveKey(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS, row.Key);
                            removed = true;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES))
                    {
                        if (row.Key == deviceId)
                        {
                            tran.RemoveKey(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES, row.Key);
                            removed = true;
                        }
                    }
                    tran.Commit();
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
                    var json = tran.Select<string, string>(DatabaseTableNames.DBREEZE_TABLE_CAMERAS, deviceId).Value;
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
                hardware.Switches = GetLazyBones();
                hardware.DanaLocks = GetDanaLocks();
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
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_CAMERAS))
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
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS))
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
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES))
                    {
                        var deviceGuid = row.Key;
                        var deviceConfig = JsonConvert.DeserializeObject<LazyBone>(row.Value);
                        lazyBones.Add(deviceConfig);
                    }
                }
                return lazyBones;
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
                Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                var exception = new Exception($"Could not get value for table: {table}, key: {key}, exception: " + ex);
                Logger.Instance.LogErrorWithContext(this.GetType(), exception);
                throw exception;
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

        private void InitDB()
        {
            if (_engine == null)
            {
                try
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    _engine = new DBreezeEngine(new DBreezeConfiguration { DBreezeDataFolderName = localFolder.Path });
                    Logger.Instance.LogInformationWithContext(this.GetType(), "Database DBreeze (settings DB) Initialized succesfully");
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                }
            }
        }

        private void InitializeSettings()
        {
            //SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "HistoryLog", 365.ToString());
            //SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureIotHubUri", Secrets.AZURE_IOT_HUB_URI);
            //SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureIotHubUriConnectionString", Secrets.AZURE_IOT_HUB_CONNECTIONSTRING);
            //SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureAccountName", Secrets.AZURE_ACCOUNT_NAME);
            //SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "AzureStorageAccessKey", Secrets.AZURE_STORAGE_ACCESS_KEY);
            //SetValue(DatabaseTableNames.DBREEZE_TABLE_SETTINGS, "DropboxAccessToken", Secrets.DROPBOX_ACCESS_TOKEN);
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
