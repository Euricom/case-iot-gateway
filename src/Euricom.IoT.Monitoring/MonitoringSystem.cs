using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Euricom.IoT.Common;
using System.Diagnostics;

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

        //public void MonitorLazyBones()
        //{
        //    //Get all lazybone configs from db 
        //    var lazyBones = DataLayer.Database.Instance.GetLazyBones();
        //    lazyBones = lazyBones.Where(x => !String.IsNullOrEmpty(x.Host) && x.Port > 0 && x.PollingTime >= 5000 && x.Enabled).ToList();

        //    foreach (var lazybone in lazyBones)
        //    {
        //        var pollingTime = lazybone.PollingTime;
        //        StartMonitor(lazybone.DeviceId);
        //    }
        //}

        //public void MonitorDanaLocks()
        //{
        //    //Get all lazybone configs from db 
        //    var danaLocks = DataLayer.Database.Instance.GetDanaLocks();
        //    danaLocks = danaLocks.Where(x => x.NodeId > 0x0 && x.Enabled).ToList();

        //    foreach (var danaLock in danaLocks)
        //    {
        //        var pollingTime = danaLock.PollingTime;
        //        StartMonitor(danaLock.DeviceId);
        //    }
        //}

        public void StartMonitor(string deviceId)
        {
            var config = DataLayer.Database.Instance.FindDevice(deviceId);
            if (config != null)
            {
                switch (config.Type)
                {
                    case Common.HardwareType.LazyBoneSwitch:
                        var configLazyBone = ((Common.LazyBone)config);
                        if (configLazyBone.Enabled && configLazyBone.PollingTime >= MIN_POLLING_TIME)
                        {
                            var ctsLazyBone = new LazyBoneMonitor().StartMonitor(configLazyBone, configLazyBone.PollingTime);
                            _cancellationTokenSources[deviceId] = ctsLazyBone;
                        }
                        break;

                    case Common.HardwareType.DanaLock:
                        var configDanaLock = ((Common.DanaLock)config);
                        if (configDanaLock.Enabled && configDanaLock.PollingTime >= MIN_POLLING_TIME)
                        {
                            var ctsDanaLock = new DanaLockMonitor().StartMonitor(configDanaLock, configDanaLock.PollingTime);
                            _cancellationTokenSources[deviceId] = ctsDanaLock;
                        }
                        break;
                }
            }
        }

        public void RemoveMonitor(string deviceId)
        {
            // Stop
            _cancellationTokenSources[deviceId].Cancel();
        }

        public void ChangePollingTime(string deviceId)
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
                        var configLazyBone = (Common.LazyBone)config;
                        new LazyBoneMonitor().StartMonitor(configLazyBone, configLazyBone.PollingTime);
                        break;

                    case Common.HardwareType.DanaLock:
                        var configDanaLock = (Common.DanaLock)config;
                        new DanaLockMonitor().StartMonitor(configDanaLock, configDanaLock.PollingTime);
                        break;
                }
            }
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
