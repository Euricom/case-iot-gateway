using Euricom.IoT.Logging;
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

        public async Task<string> TestConnection(string deviceId, Euricom.IoT.Models.LazyBone config)
        {
            try
            {
                LazyBone lazyBone = GetLazyBone(deviceId, config);
                return await lazyBone.TestConnection();

            }
            catch (Exception ex)
            {
                Logger.Instance.LogErrorWithDeviceContext(deviceId, ex);
                throw;
            }
        }

        public async Task<bool> GetCurrentState(string deviceId, Euricom.IoT.Models.LazyBone config)
        {
            LazyBone lazyBone = GetLazyBone(deviceId, config);
            return await lazyBone.GetCurrentState();
        }

        public async Task Switch(string deviceId, Euricom.IoT.Models.LazyBone config, bool state)
        {
            LazyBone lazyBone = GetLazyBone(deviceId, config);
            await lazyBone.Switch(state);
        }

        public LazyBone GetLazyBone(string deviceId, Euricom.IoT.Models.LazyBone config)
        {
            lock (_syncRoot)
            {
                if (!_lazyBones.ContainsKey(deviceId))
                {
                    var lazyBone = new LazyBone(new SocketClient(config.Host, config.Port));
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
