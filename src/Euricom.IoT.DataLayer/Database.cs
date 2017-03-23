using DBreeze;
using DBreeze.Utils;
using Euricom.IoT.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Euricom.IoT.DataLayer
{
    public class Database : IDisposable
    {
        private static readonly Database _instance = new Database();

        private static DBreeze.DBreezeEngine _engine = null;

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

        public DoorLock GetDoorLockConfig(string deviceGuid)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var json = tran.Select<string, string>("DoorLock", deviceGuid).Value;
                    return JsonConvert.DeserializeObject<DoorLock>(json);
                }
            }
            catch (Exception ex)
            {
                //TODO add logging to file ?
                throw;
            }
        }


        public Switch GetSwitchConfig(string deviceGuid)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var json = tran.Select<string, string>("Switches", deviceGuid).Value;
                    return JsonConvert.DeserializeObject<Switch>(json);
                }
            }
            catch (Exception ex)
            {
                //TODO add logging to file ?
                throw;
            }
        }

        public Camera GetCameraConfig(string deviceGuid)
        {
            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    var json = tran.Select<string, string>("Camera", deviceGuid).Value;
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
                hardware.Switches = GetSwitches();
                hardware.DoorLocks = GetDoorLocks();
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
                    foreach (var row in tran.SelectForward<string, string>("Log"))
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
                    foreach (var row in tran.SelectForward<string, string>("Cameras"))
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

        public List<DoorLock> GetDoorLocks()
        {
            try
            {
                List<DoorLock> doorlocks = new List<DoorLock>();
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>("DoorLocks"))
                    {
                        var deviceGuid = row.Key;
                        var deviceConfig = JsonConvert.DeserializeObject<DoorLock>(row.Value);
                        doorlocks.Add(deviceConfig);
                    }
                }
                return doorlocks;
            }
            catch (Exception ex)
            {
                //TODO add logging to file ?
                throw;
            }
        }

        public List<Switch> GetSwitches()
        {
            try
            {
                List<Switch> switches = new List<Switch>();
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var row in tran.SelectForward<string, string>("Switches"))
                    {
                        var deviceGuid = row.Key;
                        var deviceConfig = JsonConvert.DeserializeObject<Switch>(row.Value);
                        switches.Add(deviceConfig);
                    }
                }
                return switches;
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
