using DBreeze;
using Euricom.IoT.Common;
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
                    var json = tran.Select<string, string>("DanaLocks", deviceId).Value;
                    return JsonConvert.DeserializeObject<DanaLock>(json);
                }
            }
            catch (Exception ex)
            {
                //TODO add logging to file ?
                throw;
            }
        }

        public LazyBone GetLazyBoneConfig(string deviceId)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var json = tran.Select<string, string>("LazyBones", deviceId).Value;
                    return JsonConvert.DeserializeObject<LazyBone>(json);
                }
            }
            catch (Exception ex)
            {
                //TODO add logging to file ?
                throw;
            }
        }

        public Device FindDevice(string deviceid)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_CAMERAS))
                    {
                        if (row.Key == deviceid)
                        {
                            var camera = JsonConvert.DeserializeObject<Camera>(row.Value);
                            return camera;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS))
                    {
                        if (row.Key == deviceid)
                        {
                            var danalock = JsonConvert.DeserializeObject<DanaLock>(row.Value);
                            return danalock;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES))
                    {
                        if (row.Key == deviceid)
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
                //TODO add logging to file ?
                throw;
            }
        }

        public bool RemoveDevice(string deviceid)
        {
            try
            {
                bool removed = false;
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_CAMERAS))
                    {
                        if (row.Key == deviceid)
                        {
                            tran.RemoveKey(DatabaseTableNames.DBREEZE_TABLE_CAMERAS, row.Key);
                            removed = true;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS))
                    {
                        if (row.Key == deviceid)
                        {
                            tran.RemoveKey(DatabaseTableNames.DBREEZE_TABLE_DANALOCKS, row.Key);
                            removed = true;
                        }
                    }
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES))
                    {
                        if (row.Key == deviceid)
                        {
                            tran.RemoveKey(DatabaseTableNames.DBREEZE_TABLE_LAZYBONES, row.Key);
                            removed = true;
                        }
                    }
                }
                return removed;
            }
            catch (Exception ex)
            {
                //TODO add logging to file ?
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
                //TODO add logging to file ?
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
                //TODO add logging to file ?
                throw;
            }
        }

        public Log GetLog()
        {
            try
            {
                var log = new Log();
                List<LogLine> logs = new List<LogLine>();
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>(DatabaseTableNames.DBREEZE_TABLE_LOGGING))
                    {
                        var sequence = Int64.Parse(row.Key);
                        var logLine = JsonConvert.DeserializeObject<LogLine>(row.Value);
                        logs.Add(logLine);
                    }
                }
                log.LogLines = logs;
                return log;
            }
            catch (Exception ex)
            {
                //TODO add logging to file ?
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
                //TODO add logging to file ?
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
                //TODO add logging to file ?
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
                //TODO add logging to file ?
                throw;
            }
        }

        public void SetValue(string table, string key, string value)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    string json = JsonConvert.SerializeObject(value);
                    tran.Insert<string, string>(table, key, json);
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                //TODO add logging to file ?
                throw;
            }
        }

        private void InitDB()
        {
            if (_engine == null)
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                //test write to local folder
                //StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt");
                //await FileIO.WriteTextAsync(sampleFile, "My text");

                //test read the first line of dataFile.txt in LocalFolder and store it in a String
                //StorageFile sampleFile = await localFolder.GetFileAsync("dataFile.txt");
                //String fileContent = await FileIO.ReadTextAsync(sampleFile);

                _engine = new DBreezeEngine(new DBreezeConfiguration { DBreezeDataFolderName = localFolder.Path });
            }

            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    tran.Insert<string, string>("t1", "azure-admin", "admin");
                    tran.Insert<string, string>("t1", "azure-pass", "secret-password");
                    tran.Commit();

                    var row = tran.Select<string, string>("t1", "azure-pass").Value;
                }
            }
            catch (Exception ex)
            {
                //TODO add logging to file ?
                throw;
            }
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
