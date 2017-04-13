using DBreeze;
using Euricom.IoT.Common;
using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using Euricom.IoT.Models.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.Storage;
using System.Reactive.Linq;   // IMPORTANT - this makes await work!
using Akavache;
using System.Threading.Tasks;
using System.Text;

namespace Euricom.IoT.DataLayer
{
    public class Database : IDisposable, IDatabase
    {
        private static volatile Database _instance;
        private static object _syncRoot = new Object();
        private static DBreeze.DBreezeEngine _engine;

        private Database()
        {
            // Make sure you set the application name before doing any inserts or gets
            BlobCache.ApplicationName = "AkavacheExperiment";

            InitDB();
        }

        public static Database Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new Database();
                    }
                }

                return _instance;
            }
        }

        public void AddResetPasswordGuid(string guid)
        {
            BlobCache.LocalMachine.InsertObject("reset-password", guid);
        }

        public async Task<bool> VerifyResetPasswordGuid(string guid)
        {
            try
            {
                var key = await BlobCache.LocalMachine.Get("reset-password"); //if guid not found throws exception
                var value = Encoding.UTF8.GetString(key, 0, key.Length);
                if (value != guid)
                {
                    return false;
                }
                await BlobCache.LocalMachine.Invalidate(guid);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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

        public Settings GetConfigSettings()
        {
            var settings = new Settings();
            settings.HistoryLog = Int32.Parse(GetValue(Constants.DBREEZE_TABLE_SETTINGS, "HistoryLog"));

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

        public void SaveConfigSettings(Settings settings)
        {
            if (settings != null)
            {
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "HistoryLog", settings.HistoryLog.ToString());
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "LogLevel", settings.LogLevel.ToString());
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "GatewayDeviceKey", settings.GatewayDeviceKey.ToString());
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUri", settings.AzureIotHubUri);
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUriConnectionString", settings.AzureIotHubUriConnectionString);
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureAccountName", settings.AzureAccountName);
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureStorageAccessKey", settings.AzureStorageAccessKey);
                SetValue(Constants.DBREEZE_TABLE_SETTINGS, "DropboxAccessToken", settings.DropboxAccessToken);
            }

            if (settings != null && !String.IsNullOrEmpty(settings.Password))
            {
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
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "HistoryLog", 365.ToString());
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUri", Secrets.AZURE_IOT_HUB_URI);
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureIotHubUriConnectionString", Secrets.AZURE_IOT_HUB_CONNECTIONSTRING);
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureAccountName", Secrets.AZURE_ACCOUNT_NAME);
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "AzureStorageAccessKey", Secrets.AZURE_STORAGE_ACCESS_KEY);
            //SetValue(Constants.DBREEZE_TABLE_SETTINGS, "DropboxAccessToken", Secrets.DROPBOX_ACCESS_TOKEN);
        }

        public void Dispose()
        {
            BlobCache.Shutdown().Wait();

            if (_engine != null)
            {
                _engine.Dispose();
            }
        }
    }
}
