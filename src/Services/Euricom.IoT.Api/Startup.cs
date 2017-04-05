using AutoMapper;
using Dropbox.Api.Files;
using Euricom.IoT.Api.Manager;
using Euricom.IoT.Api.Mappings;
using Euricom.IoT.Managers;
using Euricom.IoT.ZWave;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Euricom.IoT.Api
{
    public class Startup
    {
        private DropboxManager _dropBoxManager = new DropboxManager();

        public async void Run()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<LazyBoneMappingProfile>();
                cfg.AddProfile<DanaLockMappingProfile>();
                cfg.AddProfile<CameraMappingProfile>();
            });

            // Init DanaLock
            await ZWaveManager.Instance.Initialize();
            Debug.WriteLine("OpenZWave initialized");

            // Init Webserver
            await new WebServer().InitializeWebServer();
            Debug.WriteLine("Restup Web Server initialized, listening on default port 8800");

            // Set up monitoring devices
            MonitorDevices();

            // Start a task that monitors the dropbox account for new files
            //MonitorDropBoxFolder();
        }

        private void MonitorDevices()
        {
            var monitoringSystem = Monitoring.MonitoringSystem.Instance; //Constructor will be called in class and then Init()
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
