using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public static LazyBoneConnectionManager Instance { get { return lazy.Value; } }
        private LazyBoneConnectionManager()
        {
            _lazyBones = new ConcurrentDictionary<string, LazyBone>();
        }

        public async Task<bool> TestConnection(string deviceId, Common.LazyBone config)
        {
            LazyBone lazyBone;
            //lock (GetLazyBone(deviceId, config))
            //{
                lazyBone = GetLazyBone(deviceId, config);
                _lazyBones[deviceId] = lazyBone;
            //}
            return await lazyBone.TestConnection();
        }

        public LazyBone GetLazyBone(string deviceId, Common.LazyBone config)
        {
            if (!_lazyBones.ContainsKey(deviceId))
            {
                var lazyBone = new LazyBone(new SocketClient(config.Host, config.Port.ToString()));
                return lazyBone;
            }
            else
            {
                return _lazyBones[deviceId];
            }
        }
    }
}
