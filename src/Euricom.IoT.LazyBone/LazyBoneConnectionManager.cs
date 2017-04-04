using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

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

        public async Task<string> TestConnection(string deviceId, Common.LazyBone config)
        {
            LazyBone lazyBone = GetLazyBone(deviceId, config);
            return await lazyBone.TestConnection();
        }

        public async Task<bool> GetCurrentState(string deviceId, Common.LazyBone config)
        {
            LazyBone lazyBone = GetLazyBone(deviceId, config);
            return await lazyBone.GetCurrentState();
        }

        public async Task Switch(string deviceId, Common.LazyBone config, bool state)
        {
            LazyBone lazyBone = GetLazyBone(deviceId, config);
            await lazyBone.Switch(state);
        }

        public LazyBone GetLazyBone(string deviceId, Common.LazyBone config)
        {
            lock (_syncRoot)
            {
                if (!_lazyBones.ContainsKey(deviceId))
                {
                    var lazyBone = new LazyBone(new SocketClient(config.Host, config.Port.ToString()));
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
