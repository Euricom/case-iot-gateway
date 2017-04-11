using Dropbox.Api;
using Euricom.IoT.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Euricom.IoT.DropboxCleanup
{
    public class DropboxCleanup
    {
        private static volatile DropboxCleanup _instance;
        private static object _syncRoot = new Object();

        private DropboxClientConfig _dropboxClientConfig;

        private DropboxCleanup()
        {
            _dropboxClientConfig = new DropboxClientConfig("");
        }

        public static DropboxCleanup Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new DropboxCleanup();
                    }
                }

                return _instance;
            }
        }

        public async Task Cleanup(string deviceName, int maxDays)
        {
            if (maxDays <= 0)
            {
                Logger.Instance.LogWarningWithContext(this.GetType(), "Dropbox cleaning not starting, because maxDays was less than or equal to zero");
                return;
            }

            var settings = DataLayer.Database.Instance.GetConfigSettings();
            var device = DataLayer.Database.Instance.GetCameras().SingleOrDefault(x => x.Name == deviceName);
            if (device == null)
            {
                Logger.Instance.LogWarningWithContext(this.GetType(), $"Could not find camera device with name: {deviceName}.. Dropbox cleanup was not initiated!");
                return;
            }

            var dropboxClient = new DropboxClient(settings.DropboxAccessToken, _dropboxClientConfig);

            // List folder with name Camera name
            var path = "/" + deviceName;
            var folder = await dropboxClient.Files.ListFolderAsync(path, true, false, false, false);
            if (folder.Entries.Any())
            {
                var filteredEntries = folder.Entries.Where(x => x.IsFile && x.AsFile.ServerModified.AddDays(maxDays) < DateTime.Now).ToList();
                if (filteredEntries.Any())
                    Logger.Instance.LogInformationWithDeviceContext(device.DeviceId, $"Deleting {filteredEntries.Count} files from dropbox");

                foreach (var entry in filteredEntries)
                {
                    await dropboxClient.Files.DeleteAsync(entry.PathLower);
                }
            }
        }
    }
}
