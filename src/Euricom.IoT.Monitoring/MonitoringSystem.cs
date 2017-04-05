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
        private Dictionary<string, CancellationTokenSource> _cancellationTokenSources;
        private Dictionary<string, int> _pollingTimesCache;

        private const int MIN_POLLING_TIME = 5000;

        private MonitoringSystem()
        {
            Debug.WriteLine("In Constructor Monitoring System");
            _cancellationTokenSources = new Dictionary<string, CancellationTokenSource>();
            _pollingTimesCache = new Dictionary<string, int>();
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

                    Debug.WriteLine("Monitoring System: Init GetRemovedDevices()");
                    var removedDevices = GetRemovedDevices(hardware);

                    Debug.WriteLine("Monitoring System: Init GetChangedPollingTimeDevices()");
                    var changedPollingTimeDevices = GetChangedPollingTimeDevices(hardware);

                    foreach (var d in newDevices)
                    {
                        Debug.WriteLine("Monitoring System: Starting monitor newDevices");
                        StartMonitor(d);
                    }

                    foreach (var r in removedDevices)
                    {
                        Debug.WriteLine("Monitoring System: Removing monitor removedDevices");
                        RemoveMonitor(r);
                    }

                    foreach (var c in changedPollingTimeDevices)
                    {
                        Debug.WriteLine("Monitoring System: Changing monitor changedPollingTimeDevices");
                        ChangePollingTime(c);
                    }

                    await Task.Delay(1000 * 60);
                }
            });
        }

        public void StartMonitor(string deviceId)
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
                            _cancellationTokenSources[deviceId] = ctsLazyBone;
                        }
                        break;

                    case HardwareType.DanaLock:
                        var configDanaLock = ((Common.DanaLock)config);
                        if (configDanaLock.Enabled && configDanaLock.PollingTime >= MIN_POLLING_TIME)
                        {
                            var ctsDanaLock = new DanaLockMonitor().StartMonitor(configDanaLock, configDanaLock.PollingTime);
                            _pollingTimesCache[deviceId] = configDanaLock.PollingTime;
                            _cancellationTokenSources[deviceId] = ctsDanaLock;
                        }
                        break;

                    case HardwareType.Camera:
                        var configCamera = ((Common.Camera)config);
                        if (configCamera.Enabled && configCamera.PollingTime >= MIN_POLLING_TIME)
                        {
                            var ctsCamera = new CameraMonitor().StartMonitor(configCamera, configCamera.PollingTime);
                            _pollingTimesCache[deviceId] = configCamera.PollingTime;
                            _cancellationTokenSources[deviceId] = ctsCamera;
                        }
                        break;

                    default:
                        throw new Exception("Unsupported hardware type");
                }
            }
        }

        public void RemoveMonitor(string deviceId)
        {
            // Stop
            _cancellationTokenSources[deviceId].Cancel();

            if (_pollingTimesCache.ContainsKey(deviceId))
            {
                _pollingTimesCache.Remove(deviceId);
            }
        }

        public void ChangePollingTime(string deviceId)
        {
            // Stop
            _cancellationTokenSources[deviceId].Cancel();

            // Start
            StartMonitor(deviceId);
        }

        public void ChangePollingTime(string deviceId, int pollingTime)
        {
            // Stop
            _cancellationTokenSources[deviceId].Cancel();

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

        private IList<string> GetNewDevices(Hardware hardware)
        {
            var deviceIds = hardware.Cameras.Select(x => x.DeviceId).ToList();
            deviceIds.AddRange(hardware.DanaLocks.Select(x => x.DeviceId).ToList());
            deviceIds.AddRange(hardware.Switches.Select(x => x.DeviceId).ToList());

            var keys = _cancellationTokenSources.Keys;
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
                }
            }

            foreach (var danaLock in hardware.DanaLocks)
            {
                if (danaLock.Enabled && 
                    _pollingTimesCache.ContainsKey(danaLock.DeviceId) &&
                    danaLock.PollingTime != _pollingTimesCache[danaLock.DeviceId])
                {
                    results.Add(danaLock.DeviceId);
                }
            }

            foreach (var lazyBone in hardware.Switches)
            {
                if (lazyBone.Enabled && 
                    _pollingTimesCache.ContainsKey(lazyBone.DeviceId) &&
                    lazyBone.PollingTime != _pollingTimesCache[lazyBone.DeviceId])
                {
                    results.Add(lazyBone.DeviceId);
                }
            }

            return results;
        }

        private IList<string> GetRemovedDevices(Hardware hardware)
        {
            var deviceIds = hardware.Cameras.Select(x => x.DeviceId).ToList();
            deviceIds.AddRange(hardware.DanaLocks.Select(x => x.DeviceId).ToList());
            deviceIds.AddRange(hardware.Switches.Select(x => x.DeviceId).ToList());

            var keys = _cancellationTokenSources.Keys;
            var results = keys.Except(deviceIds).ToList();
            return results;
        }
    }
}
