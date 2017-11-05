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
