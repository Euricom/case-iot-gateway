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

        public async Task Cleanup(string deviceName, int maxDays, double maxStorage)
        {
            var device = DataLayer.Database.Instance.GetCameras().SingleOrDefault(x => x.Name == deviceName);
            var settings = DataLayer.Database.Instance.GetConfigSettings();
            var dropboxClient = new DropboxClient(settings.DropboxAccessToken, _dropboxClientConfig);

            if (device == null)
            {
                Logger.Instance.LogWarningWithContext(this.GetType(), $"Could not find camera device with name: {deviceName}.. Dropbox cleanup was not initiated!");
                return;
            }

            if (maxDays <= 0 && maxStorage <= 0)
            {
                Logger.Instance.LogWarningWithContext(this.GetType(), "Dropbox cleaning not starting, because maxDays was less than or equal to zero and max storage size <= 0 GB");
                return;
            }

            var path = "/" + deviceName;
            var folder = await dropboxClient.Files.ListFolderAsync(path, true, false, false, false);

            // Delete if dropbox storage reaches limit
            var spaceUsage = await dropboxClient.Users.GetSpaceUsageAsync();
            var spaceUsed = ConvertToGB(spaceUsage.Used);
            if (spaceUsed > maxStorage)
            {
                while (spaceUsed > maxStorage)
                {
                    spaceUsed = ConvertToGB((await dropboxClient.Users.GetSpaceUsageAsync()).Used);

                    // Delete files
                    var filteredEntries = folder.Entries.Where(x => x.IsFile && !x.IsDeleted).OrderBy(x=> x.AsFile.ServerModified).ToList();
                    filteredEntries = filteredEntries.Take(50).ToList();
                    foreach(var entry in filteredEntries)
                    {
                        await dropboxClient.Files.DeleteAsync(entry.PathLower);
                    }
                }
            }

            // Delete files older than max days
            if (folder.Entries.Any())
            {
                var filteredEntries = folder.Entries.Where(x => x.IsFile && !x.IsDeleted && x.AsFile.ServerModified.AddDays(maxDays) < DateTime.Now).ToList();
                if (filteredEntries.Any())
                    Logger.Instance.LogInformationWithDeviceContext(device.DeviceId, $"Deleting {filteredEntries.Count} files from dropbox");

                foreach (var entry in filteredEntries)
                {
                    await dropboxClient.Files.DeleteAsync(entry.PathLower);
                }
            }
        }

        private double ConvertToGB(ulong used)
        {
            var kiloBytes = (used / 1024);
            var megaBytes = kiloBytes / 1024;
            var gigaBytes = megaBytes / 1024;
            return gigaBytes;
        }
    }
}
