using Euricom.IoT.Logging;
using Euricom.IoT.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Euricom.IoT.LazyBone
{
    public class LazyBoneConnectionManager
    {
        private static readonly Lazy<LazyBoneConnectionManager> lazy = new Lazy<LazyBoneConnectionManager>(() => new LazyBoneConnectionManager());
        private ConcurrentDictionary<string, LazyBone> _lazyBones;
        private readonly object _syncRoot = new object();

        public static LazyBoneConnectionManager Instance { get { return lazy.Value; } }
        private LazyBoneConnectionManager()
        {
            _lazyBones = new ConcurrentDictionary<string, LazyBone>();
        }

        public async Task<bool> TestConnection(string deviceId, Euricom.IoT.Models.LazyBone config)
        {
            try
            {
                if (!config.IsDimmer)
                {
                    var lazyBoneSwitch = (Euricom.IoT.LazyBone.LazyBoneSwitch)GetLazyBone(deviceId, config);
                    return await lazyBoneSwitch.TestConnection();
                }
                else
                {
                    var lazyBoneDimmer = (Euricom.IoT.LazyBone.LazyBoneDimmer)GetLazyBone(deviceId, config);
                    return await lazyBoneDimmer.TestConnection();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public async Task<bool> GetCurrentStateSwitch(string deviceId, Euricom.IoT.Models.LazyBone config)
        {
            var lazyBone = (Euricom.IoT.LazyBone.LazyBoneSwitch)GetLazyBone(deviceId, config);
            return await lazyBone.GetCurrentState();
        }

        public async Task<LazyBoneDimmerState> GetCurrentStateDimmer(string deviceId, Euricom.IoT.Models.LazyBone config)
        {
            var lazyBone = (Euricom.IoT.LazyBone.LazyBoneDimmer)GetLazyBone(deviceId, config);
            return await lazyBone.GetCurrentState();
        }

        public async Task Switch(string deviceId, Euricom.IoT.Models.LazyBone config, bool state)
        {
            if (!config.IsDimmer)
            {
                var lazyBoneSwitch = (Euricom.IoT.LazyBone.LazyBoneSwitch)GetLazyBone(deviceId, config);
                await lazyBoneSwitch.Switch(state);
            }
            else
            {
                var lazyBoneDimmer = (Euricom.IoT.LazyBone.LazyBoneDimmer)GetLazyBone(deviceId, config);
                if (state)
                    await lazyBoneDimmer.SetLightOn();
                else
                    await lazyBoneDimmer.SetLightOff();
            }
        }

        public async Task SetLightValue(string deviceId, Euricom.IoT.Models.LazyBone config, int lightValue)
        {
            var lazyBoneDimmer = (Euricom.IoT.LazyBone.LazyBoneDimmer)GetLazyBone(deviceId, config);
            await lazyBoneDimmer.SetLightValue(lightValue);
        }

        public Euricom.IoT.LazyBone.LazyBone GetLazyBone(string deviceId, Euricom.IoT.Models.LazyBone config)
        {
            lock (_syncRoot)
            {
                if (!_lazyBones.ContainsKey(deviceId))
                {
                    LazyBone lazyBone;
                    if (!config.IsDimmer)
                    {
                        lazyBone = new LazyBoneSwitch(new SocketClient(config.Host, config.Port));
                    }
                    else
                    {
                        lazyBone = new LazyBoneDimmer(new SocketClient(config.Host, config.Port));
                    }

                    _lazyBones[deviceId] = lazyBone;
                    Debug.WriteLine("New lazybone created");
                    return lazyBone;
                }
                else
                {
                    Debug.WriteLine("Getting previous lazybone");
                    return _lazyBones[deviceId];
                }
            }
        }
    }
}
