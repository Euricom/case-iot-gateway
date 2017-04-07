using Dropbox.Api.Files;
using Euricom.IoT.Api.Manager;
using Euricom.IoT.Logging;
using Euricom.IoT.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Euricom.IoT.Monitoring
{
    public class CameraMonitor
    {
        private DropboxManager _dropBoxManager = new DropboxManager();

        public CameraMonitor()
        {

        }

        public CancellationTokenSource StartMonitor(Common.Camera camera, int pollingTime)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var entries = await _dropBoxManager.PollDropboxNewFiles(camera.DropboxPath);
                        if (entries != null && entries.Count > 0)
                        {
                            var files = await _dropBoxManager.DownloadFiles(entries);
                            new CameraManager().UploadFilesToBlobStorage(camera.DropboxPath, files);
                        }
                        await Task.Delay(pollingTime);

                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                        Logger.Instance.LogErrorWithDeviceContext(camera.DeviceId, ex);
                        Debug.WriteLine($"Exception occurred while monitoring Camera device {camera.DeviceId}, exception message: {ex.Message}");
                    }
                }
            }, ct);

            return cts;
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
