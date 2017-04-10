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
            var dropboxClient = new DropboxClient(settings.DropboxAccessToken, _dropboxClientConfig);

            // List folder with name Camera name
            var path = "/" + deviceName;
            var folder = await dropboxClient.Files.ListFolderAsync(path, true, false, false, false);
            if (folder.Entries.Any())
            {
                foreach (var entry in folder.Entries.Where(x => x.IsFile))
                {
                    var serverModified = entry.AsFile.ServerModified;
                    if (serverModified.AddDays(maxDays) < DateTime.Now)
                    {
                        await dropboxClient.Files.DeleteAsync(entry.PathLower);
                    }
                }
            }
        }
    }
}
