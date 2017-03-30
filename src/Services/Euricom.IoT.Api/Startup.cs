using Dropbox.Api.Files;
using Euricom.IoT.Api.Manager;
using Euricom.IoT.Common.Secrets;
using Euricom.IoT.Devices.DanaLock;
using Euricom.IoT.Managers;
using Euricom.IoT.Monitoring;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Euricom.IoT.Api
{
    public class Startup
    {
        private DanaLockMonitor _danaLockMonitor = new DanaLockMonitor();
        private LazyBoneMonitor _lazyBoneMonitor = new LazyBoneMonitor();
        private DropboxManager _dropBoxManager = new DropboxManager();


        public async void Run()
        {
            // Init DanaLock
            await DanaLock.Instance.Initialize();

            // Init Webserver
            await new WebServer().InitializeWebServer();

            // Set up monitoring devices
            //MonitorDanaLocks();
            //MonitorLazyBones();

            // Start a task that monitors the dropbox account for new files
            //MonitorDropBoxFolder();
        }

        private void MonitorDanaLocks()
        {
            //Get all danalocks configs from db (nodeIds)
            //var nodeIds = new List<byte>() { 0x4 };
            //var danalocks = DataLayer.Database.Instance.GetDanaLocks();

            //foreach (var danalock in danalocks)
            //{ 
            //    var pollingTime = 5000; // TODO: get from config
            //    _danaLockMonitor.StartMonitor(danalock.DeviceId, pollingTime);
            //}

            var pollingTime = 5000; // TODO: get from config
            _danaLockMonitor.StartMonitor(Secrets.DANALOCK_DEVICE_KEY, pollingTime);
        }

        private void MonitorLazyBones()
        {
            //Get all lazybone configs from db (nodeIds)
            //var lazyBones = DataLayer.Database.Instance.GetLazyBones();

            //foreach (var lazybone in danalocks)
            //{ 
            //    var pollingTime = 5000; // TODO: get from config
            //    _danaLockMonitor.StartMonitor(danalock.DeviceId, pollingTime);
            //}

            var pollingTime = 5000; // TODO: get from config
            _lazyBoneMonitor.StartMonitor("todo fill in", pollingTime);
        }

        private void MonitorDropBoxFolder()
        {
            var pollingTime = 1000 * 30; // TODO: get from config

            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Task.Run(async () =>
            {
                while (true)
                {
                    var entries = await _dropBoxManager.PollDropboxNewFiles();
                    if (entries != null && entries.Count > 0)
                    {
                        var files = await _dropBoxManager.DownloadFiles(entries);
                        new CameraManager().UploadFilesToBlobStorage(files);
                    }
                    await Task.Delay(pollingTime);
                }
            }, ct);
        }

        private List<string> GetFileNames(IList<Metadata> entries)
        {
            List<string> results = new List<string>();
            foreach (var entry in entries)
            {
                if (entry.IsFile && !entry.IsDeleted)
                {
                    results.Add(entry.Name);
                }
            }
            return results;
        }
    }
}
