using Euricom.IoT.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Euricom.IoT.Monitoring
{
    public class MonitoringSystem
    {
        private static readonly MonitoringSystem _instance = new MonitoringSystem();

        // Cache of polling time per device
        private Dictionary<string, int> _pollingTimesCache;
        private Dictionary<string, int> _cacheCameraMaxDaysDropbox;
        private Dictionary<string, int> _cacheCameraMaxDaysAzureBlobStorage;

        // Cancellation token source per device
        private Dictionary<string, CancellationTokenSource> _cancellationDevicesPolling;
        private CancellationTokenSource _cancellationDropboxCleanup;
        private CancellationTokenSource _cancellationAzureCleanup;

        private const int MIN_POLLING_TIME = 5000;
        private const int DROPBOX_CLEANUP_CHECK_INTERVAl = 1000 * 60 * 30;
        private const int AZURE_CLEANUP_CHECK_INTERVAl = 1000 * 60 * 30;

        private MonitoringSystem()
        {
            _cancellationDevicesPolling = new Dictionary<string, CancellationTokenSource>();
            _pollingTimesCache = new Dictionary<string, int>();
            _cacheCameraMaxDaysDropbox = new Dictionary<string, int>();
            _cacheCameraMaxDaysAzureBlobStorage = new Dictionary<string, int>();
            Init();
        }

        public static MonitoringSystem Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Init()
        {
            Debug.WriteLine("Monitoring System: Init()");

            MonitorDevices();

            CleanupDropboxFolder(DROPBOX_CLEANUP_CHECK_INTERVAl);

            CleanupAzureBlobStorage(AZURE_CLEANUP_CHECK_INTERVAl);
        }

        public void StartDevicePollingMonitor(string deviceId)
        {
            var config = DataLayer.Database.Instance.FindDevice(deviceId);
            if (config != null)
            {
                switch (config.Type)
                {
                    case HardwareType.LazyBoneSwitch:
                        var configLazyBone = ((Common.LazyBone)config);
                        if (configLazyBone.Enabled && configLazyBone.PollingTime >= MIN_POLLING_TIME)
                        {
                            var ctsLazyBone = new LazyBoneMonitor().StartMonitor(configLazyBone, configLazyBone.PollingTime);
                            _pollingTimesCache[deviceId] = configLazyBone.PollingTime;
                            _cancellationDevicesPolling[deviceId] = ctsLazyBone;
                        }
                        break;

                    case HardwareType.DanaLock:
                        var configDanaLock = ((Common.DanaLock)config);
                        if (configDanaLock.Enabled && configDanaLock.PollingTime >= MIN_POLLING_TIME)
                        {
                            var ctsDanaLock = new DanaLockMonitor().StartMonitor(configDanaLock, configDanaLock.PollingTime);
                            _pollingTimesCache[deviceId] = configDanaLock.PollingTime;
                            _cancellationDevicesPolling[deviceId] = ctsDanaLock;
                        }
                        break;

                    case HardwareType.Camera:
                        var configCamera = ((Common.Camera)config);
                        if (configCamera.Enabled && configCamera.PollingTime >= MIN_POLLING_TIME)
                        {
                            var ctsCamera = new CameraMonitor().StartMonitor(configCamera, configCamera.PollingTime);
                            _pollingTimesCache[deviceId] = configCamera.PollingTime;
                            _cacheCameraMaxDaysDropbox[deviceId] = configCamera.MaximumDaysDropbox;
                            _cancellationDevicesPolling[deviceId] = ctsCamera;
                        }
                        break;

                    default:
                        throw new Exception("Unsupported hardware type");
                }
            }
        }

        public void RemoveDevicePollingMonitor(string deviceId)
        {
            // Stop
            _cancellationDevicesPolling[deviceId].Cancel();

            // Clear caches
            if (_pollingTimesCache.ContainsKey(deviceId))
            {
                _pollingTimesCache.Remove(deviceId);
            }

            // Clear caches
            if (_cacheCameraMaxDaysDropbox.ContainsKey(deviceId))
            {
                _cacheCameraMaxDaysDropbox.Remove(deviceId);
            }

            // Clear caches
            if (_cacheCameraMaxDaysAzureBlobStorage.ContainsKey(deviceId))
            {
                _cacheCameraMaxDaysAzureBlobStorage.Remove(deviceId);
            }
        }

        public void ChangePollingTime(string deviceId)
        {
            // Stop
            _cancellationDevicesPolling[deviceId].Cancel();

            // Start
            StartDevicePollingMonitor(deviceId);
        }

        public void ChangePollingTime(string deviceId, int pollingTime)
        {
            // Stop
            _cancellationDevicesPolling[deviceId].Cancel();

            // Start
            var config = DataLayer.Database.Instance.FindDevice(deviceId);
            if (config != null)
            {
                switch (config.Type)
                {
                    case Common.HardwareType.LazyBoneSwitch:
                        new LazyBoneMonitor().StartMonitor((Common.LazyBone)config, pollingTime);
                        break;

                    case Common.HardwareType.DanaLock:
                        new DanaLockMonitor().StartMonitor((Common.DanaLock)config, pollingTime);
                        break;
                }
            }
        }

        public void ChangeDropboxCleanupParameter(string deviceId)
        {
            _cancellationDropboxCleanup.Cancel();

            CleanupDropboxFolder(DROPBOX_CLEANUP_CHECK_INTERVAl);
        }

        public void ChangeAzureCleanupParameter(string deviceId)
        {
            _cancellationAzureCleanup.Cancel();

            CleanupDropboxFolder(DROPBOX_CLEANUP_CHECK_INTERVAl);
        }

        private IList<string> GetNewDevices(Hardware hardware)
        {
            var deviceIds = hardware.Cameras.Select(x => x.DeviceId).ToList();
            deviceIds.AddRange(hardware.DanaLocks.Select(x => x.DeviceId).ToList());
            deviceIds.AddRange(hardware.Switches.Select(x => x.DeviceId).ToList());

            var keys = _cancellationDevicesPolling.Keys;
            var results = deviceIds.Except(keys).ToList();
            return results;
        }

        private IList<string> GetChangedPollingTimeDevices(Hardware hardware)
        {
            var results = new List<string>();

            foreach (var camera in hardware.Cameras)
            {
                if (camera.Enabled &&
                    _pollingTimesCache.ContainsKey(camera.DeviceId) &&
                    camera.PollingTime != _pollingTimesCache[camera.DeviceId])
                {
                    results.Add(camera.DeviceId);
                    _pollingTimesCache[camera.DeviceId] = camera.PollingTime;
                }
            }

            foreach (var danaLock in hardware.DanaLocks)
            {
                if (danaLock.Enabled &&
                    _pollingTimesCache.ContainsKey(danaLock.DeviceId) &&
                    danaLock.PollingTime != _pollingTimesCache[danaLock.DeviceId])
                {
                    results.Add(danaLock.DeviceId);
                    _pollingTimesCache[danaLock.DeviceId] = danaLock.PollingTime;
                }
            }

            foreach (var lazyBone in hardware.Switches)
            {
                if (lazyBone.Enabled &&
                    _pollingTimesCache.ContainsKey(lazyBone.DeviceId) &&
                    lazyBone.PollingTime != _pollingTimesCache[lazyBone.DeviceId])
                {
                    results.Add(lazyBone.DeviceId);
                    _pollingTimesCache[lazyBone.DeviceId] = lazyBone.PollingTime;
                }
            }

            return results;
        }

        private IList<string> GetChangedCamerasMaxDaysDropbox(Hardware hardware)
        {
            var results = new List<string>();
            foreach (var camera in hardware.Cameras)
            {
                if (camera.Enabled &&
                    _cacheCameraMaxDaysDropbox.ContainsKey(camera.DeviceId) &&
                    camera.MaximumDaysDropbox != _cacheCameraMaxDaysDropbox[camera.DeviceId])
                {
                    Logging.Logger.Instance.LogInformationWithDeviceContext(camera.DeviceId, $"Changed camera settings cleanup for dropbox to {camera.MaximumDaysAzureBlobStorage} days");
                    results.Add(camera.DeviceId);
                }
            }
            return results;
        }

        private IList<string>  GetChangedCamerasMaxDaysAzure(Hardware hardware)
        {
            var results = new List<string>();
            foreach (var camera in hardware.Cameras)
            {
                if (camera.Enabled &&
                    _cacheCameraMaxDaysAzureBlobStorage.ContainsKey(camera.DeviceId) &&
                    camera.MaximumDaysAzureBlobStorage != _cacheCameraMaxDaysAzureBlobStorage[camera.DeviceId])
                {
                    Logging.Logger.Instance.LogInformationWithDeviceContext(camera.DeviceId, $"Changed camera settings cleanup for azure blob storage to {camera.MaximumDaysAzureBlobStorage} days");
                    results.Add(camera.DeviceId);
                }
            }
            return results;
        }

        private IList<string> GetRemovedDevices(Hardware hardware)
        {
            var deviceIds = hardware.Cameras.Select(x => x.DeviceId).ToList();
            deviceIds.AddRange(hardware.DanaLocks.Select(x => x.DeviceId).ToList());
            deviceIds.AddRange(hardware.Switches.Select(x => x.DeviceId).ToList());

            var keys = _cancellationDevicesPolling.Keys;
            var results = keys.Except(deviceIds).ToList();
            return results;
        }

        private void MonitorDevices()
        {
            // Regularly check whether a device was added, removed or polling time was changed
            Task.Run(async () =>
            {
                while (true)
                {
                    Debug.WriteLine("Monitoring System: Init in while true()");

                    Debug.WriteLine("Monitoring System: Init GetHardware()");

                    var hardware = DataLayer.Database.Instance.GetHardware();

                    Debug.WriteLine("Monitoring System: Init GetNewDevices()");
                    var newDevices = GetNewDevices(hardware);

                    Debug.WriteLine("Monitoring System: GetRemovedDevices()");
                    var removedDevices = GetRemovedDevices(hardware);

                    Debug.WriteLine("Monitoring System: GetChangedPollingTimeDevices()");
                    var changedPollingTimeDevices = GetChangedPollingTimeDevices(hardware);

                    Debug.WriteLine("Monitoring System: GetChangedCamerasMaxDaysDropbox()");
                    var changedCamerasMaxDaysDropbox = GetChangedCamerasMaxDaysDropbox(hardware);

                    Debug.WriteLine("Monitoring System: GetChangedCamerasMaxDaysAzure()");
                    var changedCamerasMaxDaysAzure = GetChangedCamerasMaxDaysAzure(hardware);

                    foreach (var d in newDevices)
                    {
                        Debug.WriteLine("Monitoring System: Starting monitor newDevices");
                        StartDevicePollingMonitor(d);
                    }

                    foreach (var r in removedDevices)
                    {
                        Debug.WriteLine("Monitoring System: Removing monitor removedDevices");
                        RemoveDevicePollingMonitor(r);
                    }

                    foreach (var c in changedPollingTimeDevices)
                    {
                        Debug.WriteLine("Monitoring System: Changing monitor changedPollingTimeDevices");
                        ChangePollingTime(c);
                    }

                    foreach (var cameraDeviceId in changedCamerasMaxDaysDropbox)
                    {
                        ChangeDropboxCleanupParameter(cameraDeviceId);
                    }

                    foreach (var cameraDeviceId2 in changedCamerasMaxDaysAzure)
                    {
                        ChangeAzureCleanupParameter(cameraDeviceId2);
                    }


                    // Wait 60 seconds
                    await Task.Delay(1000 * 60);
                }
            });
        }

        private void CleanupDropboxFolder(int waitInterval)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var cameras = DataLayer.Database.Instance.GetCameras().ToList();
                        foreach (var camera in cameras)
                        {
                            Debug.WriteLine($"Cleaning dropbox for camera {camera.DeviceId}");
                            Logging.Logger.Instance.LogInformationWithDeviceContext(camera.DeviceId, $"Cleaning dropbox for camera (max days was {camera.MaximumDaysDropbox})");

                            // Cleanup 
                            await DropboxCleanup.DropboxCleanup.Instance.Cleanup(camera.Name, camera.MaximumDaysDropbox);

                            Logging.Logger.Instance.LogInformationWithDeviceContext(camera.DeviceId, $"Cleaning dropbox for camera completed");
                            Debug.WriteLine($"Cleaning dropbox for camera {camera.DeviceId} completed");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                    }

                    // Wait 30 minutes
                    await Task.Delay(1000 * 60 * 30);
                }
            }, ct);

            _cancellationDropboxCleanup = cts;
        }

        private void CleanupAzureBlobStorage(int waitInterval)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var cameras = DataLayer.Database.Instance.GetCameras().ToList();
                        foreach (var camera in cameras)
                        {
                            Debug.WriteLine($"Cleaning azure blob storage for camera {camera.DeviceId}");
                            Logging.Logger.Instance.LogInformationWithDeviceContext(camera.DeviceId, $"Cleaning azure blob storage for camera (max days was {camera.MaximumDaysDropbox})");

                            // Cleanup 
                            await new AzureBlobStorage.AzureBlobStorageManager().Cleanup(camera.Name, camera.MaximumDaysAzureBlobStorage);

                            Logging.Logger.Instance.LogInformationWithDeviceContext(camera.DeviceId, $"Cleaning azure blob storage for camera completed");
                            Debug.WriteLine($"Cleaning azure blob storage for camera {camera.DeviceId} completed");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                    }

                    // Wait 30 minutes
                    await Task.Delay(1000 * 60 * 30);
                }
            }, ct);

            _cancellationAzureCleanup = cts;
        }
    }
}
