//using Euricom.IoT.Models;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Euricom.IoT.AzureBlobStorage;
//using Euricom.IoT.DataLayer;

//namespace Euricom.IoT.Monitoring
//{
//    public class MonitoringSystem
//    {
//        private readonly Database _database;
//        private readonly IAzureBlobStorageManager _blobStorageManager;

//        // Cache of polling time per device
//        private Dictionary<string, int> _pollingTimesCache;
//        private Dictionary<string, int> _cacheCameraMaxDaysDropbox;
//        private Dictionary<string, int> _cacheCameraMaxDaysAzureBlobStorage;

//        // Cancellation token source per device
//        private Dictionary<string, CancellationTokenSource> _cancellationDevicesPolling;
//        private CancellationTokenSource _cancellationDropboxCleanup;
//        private CancellationTokenSource _cancellationAzureCleanup;

//        private const int MIN_POLLING_TIME = 5000;
//        private const int DROPBOX_CLEANUP_CHECK_INTERVAl = 1000 * 60 * 30;
//        private const int AZURE_CLEANUP_CHECK_INTERVAl = 1000 * 60 * 30;

//        private MonitoringSystem(Database database, IAzureBlobStorageManager blobStorageManager)
//        {
//            _database = database;
//            _blobStorageManager = blobStorageManager;
//            _cancellationDevicesPolling = new Dictionary<string, CancellationTokenSource>();
//            _pollingTimesCache = new Dictionary<string, int>();
//            _cacheCameraMaxDaysDropbox = new Dictionary<string, int>();
//            _cacheCameraMaxDaysAzureBlobStorage = new Dictionary<string, int>();

//            Init();
//        }
        
//        private void Init()
//        {
//            Debug.WriteLine("Monitoring System: Init()");

//            MonitorDevices();

//            CleanupCameraDropboxFolders(DROPBOX_CLEANUP_CHECK_INTERVAl);

//            CleanupAzureBlobStorage(AZURE_CLEANUP_CHECK_INTERVAl);
//        }

//        public void StartDevicePollingMonitor(string deviceId)
//        {
//            var config = _database.FindDevice(deviceId);
//            if (config != null)
//            {
//                switch (config.Type)
//                {
//                    case HardwareType.LazyBoneSwitch:
//                        //var configLazyBoneSwitch = ((Euricom.IoT.Models.LazyBone)config);
//                        //if (configLazyBoneSwitch.Enabled && configLazyBoneSwitch.PollingTime >= MIN_POLLING_TIME)
//                        //{
//                        //    var ctsLazyBone = new LazyBoneMonitor().StartMonitor(configLazyBoneSwitch, configLazyBoneSwitch.PollingTime);
//                        //    _pollingTimesCache[deviceId] = configLazyBoneSwitch.PollingTime;
//                        //    _cancellationDevicesPolling[deviceId] = ctsLazyBone;
//                        //}
//                        break;

//                    case HardwareType.LazyBoneDimmer:
//                        //var configLazyBoneDimmer = ((Euricom.IoT.Models.LazyBone)config);
//                        //if (configLazyBoneDimmer.Enabled && configLazyBoneDimmer.PollingTime >= MIN_POLLING_TIME)
//                        //{
//                        //    var ctsLazyBone = new LazyBoneMonitor().StartMonitor(configLazyBoneDimmer, configLazyBoneDimmer.PollingTime);
//                        //    _pollingTimesCache[deviceId] = configLazyBoneDimmer.PollingTime;
//                        //    _cancellationDevicesPolling[deviceId] = ctsLazyBone;
//                        //}
//                        break;

//                    case HardwareType.DanaLock:
//                        var configDanaLock = ((Euricom.IoT.Models.DanaLock)config);
//                        if (configDanaLock.Enabled && configDanaLock.PollingTime >= MIN_POLLING_TIME)
//                        {
//                            var ctsDanaLock = new DanaLockMonitor().StartMonitor(configDanaLock, configDanaLock.PollingTime);
//                            _pollingTimesCache[deviceId] = configDanaLock.PollingTime;
//                            _cancellationDevicesPolling[deviceId] = ctsDanaLock;
//                        }
//                        break;

//                    case HardwareType.WallMountSwitch:
//                        var configWallmount = ((Euricom.IoT.Models.WallMountSwitch)config);
//                        if (configWallmount.Enabled && configWallmount.PollingTime >= MIN_POLLING_TIME)
//                        {
//                            var ctsWallmount = new WallmountMonitor().StartMonitor(configWallmount, configWallmount.PollingTime);
//                            _pollingTimesCache[deviceId] = configWallmount.PollingTime;
//                            _cancellationDevicesPolling[deviceId] = ctsWallmount;
//                        }
//                        break;

//                    case HardwareType.Camera:
//                        var configCamera = ((Camera)config);
//                        if (configCamera.Enabled && configCamera.PollingTime >= MIN_POLLING_TIME)
//                        {
//                            var ctsCamera = new CameraMonitor().StartMonitor(configCamera, configCamera.PollingTime);
//                            _pollingTimesCache[deviceId] = configCamera.PollingTime;
//                            _cacheCameraMaxDaysDropbox[deviceId] = configCamera.MaximumDaysDropbox;
//                            _cancellationDevicesPolling[deviceId] = ctsCamera;
//                        }
//                        break;

//                    default:
//                        throw new Exception("Unsupported hardware type");
//                }
//            }
//        }

//        public void RemoveDevicePollingMonitor(string deviceId)
//        {
//            // Stop
//            _cancellationDevicesPolling[deviceId].Cancel();

//            // Clear caches
//            if (_pollingTimesCache.ContainsKey(deviceId))
//            {
//                _pollingTimesCache.Remove(deviceId);
//            }

//            // Clear caches
//            if (_cacheCameraMaxDaysDropbox.ContainsKey(deviceId))
//            {
//                _cacheCameraMaxDaysDropbox.Remove(deviceId);
//            }

//            // Clear caches
//            if (_cacheCameraMaxDaysAzureBlobStorage.ContainsKey(deviceId))
//            {
//                _cacheCameraMaxDaysAzureBlobStorage.Remove(deviceId);
//            }
//        }

//        public void ChangePollingTime(string deviceId)
//        {
//            // Stop
//            _cancellationDevicesPolling[deviceId].Cancel();

//            // Start
//            StartDevicePollingMonitor(deviceId);
//        }

//        public void ChangePollingTime(string deviceId, int pollingTime)
//        {
//            // Stop
//            _cancellationDevicesPolling[deviceId].Cancel();

//            // Start
//            var config = _database.FindDevice(deviceId);
//            if (config != null)
//            {
//                // TODO: factories
//                switch (config.Type)
//                {
//                    case HardwareType.LazyBoneSwitch:
//                        //new LazyBoneMonitor().StartMonitor((Euricom.IoT.Models.LazyBone)config, pollingTime);
//                        break;

//                    case HardwareType.LazyBoneDimmer:
//                        //new LazyBoneMonitor().StartMonitor((Euricom.IoT.Models.LazyBone)config, pollingTime);
//                        break;

//                    case HardwareType.DanaLock:
//                        new DanaLockMonitor().StartMonitor((Euricom.IoT.Models.DanaLock)config, pollingTime);
//                        break;

//                    case HardwareType.WallMountSwitch:
//                        new WallmountMonitor(_database).StartMonitor((Euricom.IoT.Models.WallMountSwitch)config, pollingTime);
//                        break;
//                }
//            }
//        }

//        public void ChangeDropboxCleanupParameter(string deviceId)
//        {
//            _cancellationDropboxCleanup.Cancel();

//            CleanupCameraDropboxFolders(DROPBOX_CLEANUP_CHECK_INTERVAl);
//        }

//        public void ChangeAzureCleanupParameter(string deviceId)
//        {
//            _cancellationAzureCleanup.Cancel();

//            CleanupCameraDropboxFolders(DROPBOX_CLEANUP_CHECK_INTERVAl);
//        }

//        private IList<string> GetNewDevices(Hardware hardware)
//        {
//            var deviceIds = hardware.Cameras.Select(x => x.DeviceId).ToList();
//            deviceIds.AddRange(hardware.DanaLocks.Select(x => x.DeviceId).ToList());
//            deviceIds.AddRange(hardware.LazyBones.Select(x => x.DeviceId).ToList());
//            deviceIds.AddRange(hardware.WallMountSwitches.Select(x => x.DeviceId).ToList());

//            var keys = _cancellationDevicesPolling.Keys;
//            var results = deviceIds.Except(keys).ToList();
//            return results;
//        }

//        private IList<string> GetChangedPollingTimeDevices(Hardware hardware)
//        {
//            var results = new List<string>();

//            foreach (var camera in hardware.Cameras)
//            {
//                if (camera.Enabled &&
//                    _pollingTimesCache.ContainsKey(camera.DeviceId) &&
//                    camera.PollingTime != _pollingTimesCache[camera.DeviceId])
//                {
//                    results.Add(camera.DeviceId);
//                    _pollingTimesCache[camera.DeviceId] = camera.PollingTime;
//                }
//            }


//            foreach (var lazyBone in hardware.LazyBones)
//            {
//                if (lazyBone.Enabled &&
//                    _pollingTimesCache.ContainsKey(lazyBone.DeviceId) &&
//                    lazyBone.PollingTime != _pollingTimesCache[lazyBone.DeviceId])
//                {
//                    results.Add(lazyBone.DeviceId);
//                    _pollingTimesCache[lazyBone.DeviceId] = lazyBone.PollingTime;
//                }
//            }

//            foreach (var danaLock in hardware.DanaLocks)
//            {
//                if (danaLock.Enabled &&
//                    _pollingTimesCache.ContainsKey(danaLock.DeviceId) &&
//                    danaLock.PollingTime != _pollingTimesCache[danaLock.DeviceId])
//                {
//                    results.Add(danaLock.DeviceId);
//                    _pollingTimesCache[danaLock.DeviceId] = danaLock.PollingTime;
//                }
//            }

//            foreach (var wallmountSwitch in hardware.WallMountSwitches)
//            {
//                if (wallmountSwitch.Enabled &&
//                    _pollingTimesCache.ContainsKey(wallmountSwitch.DeviceId) &&
//                    wallmountSwitch.PollingTime != _pollingTimesCache[wallmountSwitch.DeviceId])
//                {
//                    results.Add(wallmountSwitch.DeviceId);
//                    _pollingTimesCache[wallmountSwitch.DeviceId] = wallmountSwitch.PollingTime;
//                }
//            }


//            return results;
//        }

//        private IList<string> GetChangedCamerasMaxDaysDropbox(Hardware hardware)
//        {
//            var results = new List<string>();
//            foreach (var camera in hardware.Cameras)
//            {
//                if (camera.Enabled &&
//                    _cacheCameraMaxDaysDropbox.ContainsKey(camera.DeviceId) &&
//                    camera.MaximumDaysDropbox != _cacheCameraMaxDaysDropbox[camera.DeviceId])
//                {
//                    Logging.Logger.Instance.LogInformationWithDeviceContext(camera.DeviceId, $"Changed camera settings cleanup for dropbox to {camera.MaximumDaysAzureBlobStorage} days");
//                    results.Add(camera.DeviceId);
//                }
//            }
//            return results;
//        }

//        private IList<string>  GetChangedCamerasMaxDaysAzure(Hardware hardware)
//        {
//            var results = new List<string>();
//            foreach (var camera in hardware.Cameras)
//            {
//                if (camera.Enabled &&
//                    _cacheCameraMaxDaysAzureBlobStorage.ContainsKey(camera.DeviceId) &&
//                    camera.MaximumDaysAzureBlobStorage != _cacheCameraMaxDaysAzureBlobStorage[camera.DeviceId])
//                {
//                    Logging.Logger.Instance.LogInformationWithDeviceContext(camera.DeviceId, $"Changed camera settings cleanup for azure blob storage to {camera.MaximumDaysAzureBlobStorage} days");
//                    results.Add(camera.DeviceId);
//                }
//            }
//            return results;
//        }

//        private IList<string> GetRemovedDevices(Hardware hardware)
//        {
//            var deviceIds = hardware.Cameras.Select(x => x.DeviceId).ToList();
//            deviceIds.AddRange(hardware.DanaLocks.Select(x => x.DeviceId).ToList());
//            deviceIds.AddRange(hardware.LazyBones.Select(x => x.DeviceId).ToList());
//            deviceIds.AddRange(hardware.WallMountSwitches.Select(x => x.DeviceId).ToList());

//            var keys = _cancellationDevicesPolling.Keys;
//            var results = keys.Except(deviceIds).ToList();
//            return results;
//        }

//        private void MonitorDevices()
//        {
//            // Regularly check whether a device was added, removed or polling time was changed
//            Task.Run(async () =>
//            {
//                while (true)
//                {
//                    Debug.WriteLine("Monitoring System: Init in while true()");

//                    Debug.WriteLine("Monitoring System: Init GetHardware()");

//                    var hardware = _database.GetHardware();

//                    Debug.WriteLine("Monitoring System: Init GetNewDevices()");
//                    var newDevices = GetNewDevices(hardware);

//                    Debug.WriteLine("Monitoring System: GetRemovedDevices()");
//                    var removedDevices = GetRemovedDevices(hardware);

//                    Debug.WriteLine("Monitoring System: GetChangedPollingTimeDevices()");
//                    var changedPollingTimeDevices = GetChangedPollingTimeDevices(hardware);

//                    Debug.WriteLine("Monitoring System: GetChangedCamerasMaxDaysDropbox()");
//                    var changedCamerasMaxDaysDropbox = GetChangedCamerasMaxDaysDropbox(hardware);

//                    Debug.WriteLine("Monitoring System: GetChangedCamerasMaxDaysAzure()");
//                    var changedCamerasMaxDaysAzure = GetChangedCamerasMaxDaysAzure(hardware);

//                    foreach (var d in newDevices)
//                    {
//                        Debug.WriteLine("Monitoring System: Starting monitor newDevices");
//                        StartDevicePollingMonitor(d);
//                    }

//                    foreach (var r in removedDevices)
//                    {
//                        Debug.WriteLine("Monitoring System: Removing monitor removedDevices");
//                        RemoveDevicePollingMonitor(r);
//                    }

//                    foreach (var c in changedPollingTimeDevices)
//                    {
//                        Debug.WriteLine("Monitoring System: Changing monitor changedPollingTimeDevices");
//                        ChangePollingTime(c);
//                    }

//                    foreach (var cameraDeviceId in changedCamerasMaxDaysDropbox)
//                    {
//                        ChangeDropboxCleanupParameter(cameraDeviceId);
//                    }

//                    foreach (var cameraDeviceId2 in changedCamerasMaxDaysAzure)
//                    {
//                        ChangeAzureCleanupParameter(cameraDeviceId2);
//                    }


//                    // Wait 60 seconds
//                    await Task.Delay(1000 * 60);
//                }
//            });
//        }

//        private void CleanupCameraDropboxFolders(int waitInterval)
//        {
//            var cts = new CancellationTokenSource();
//            var ct = cts.Token;

//            Task.Run(async () =>
//            {
//                while (true)
//                {
//                    try
//                    {
//                        var cameras = _database.GetCameras().ToList();
//                        foreach (var camera in cameras)
//                        {
//                            Logging.Logger.Instance.LogDebugWithDeviceContext(camera.DeviceId, $"Cleaning dropbox for camera (if there are old files..)");

//                            // Cleanup 
//                            await DropboxCleanup.DropboxCleanup.Instance.Cleanup(camera.Name, camera.MaximumDaysDropbox, camera.MaximumStorageDropbox);

//                            Logging.Logger.Instance.LogDebugWithDeviceContext(camera.DeviceId, $"Cleaning dropbox for camera completed");
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
//                    }

//                    // Wait 30 minutes
//                    await Task.Delay(1000 * 60 * 30);
//                }
//            }, ct);

//            _cancellationDropboxCleanup = cts;
//        }

//        private void CleanupAzureBlobStorage(int waitInterval)
//        {
//            var cts = new CancellationTokenSource();
//            var ct = cts.Token;

//            Task.Run(async () =>
//            {
//                while (true)
//                {
//                    try
//                    {
//                        var cameras = _database.GetCameras().ToList();
//                        foreach (var camera in cameras)
//                        {
//                            Logging.Logger.Instance.LogDebugWithDeviceContext(camera.DeviceId, $"Cleaning azure blob storage for camera (if there are old files..)");

//                            // Cleanup 
//                            await _blobStorageManager.CleanupAsync(camera.Name, camera.MaximumDaysAzureBlobStorage);

//                            Logging.Logger.Instance.LogDebugWithDeviceContext(camera.DeviceId, $"Cleaning azure blob storage for camera completed");
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
//                    }

//                    // Wait 30 minutes
//                    await Task.Delay(1000 * 60 * 30);
//                }
//            }, ct);

//            _cancellationAzureCleanup = cts;
//        }
//    }
//}
