using System.Collections.Generic;
using Dropbox.Api.Files;
using System.Threading.Tasks;
using System.IO;

namespace Euricom.IoT.Managers.Interfaces
{
    public interface IDropboxManager
    {
        Task<Dictionary<string, byte[]>> DownloadFiles(IList<Metadata> entries);
        Task<IList<Metadata>> PollDropboxNewFiles();
        //void StartMonitor();
        //void StartMonitor(int pollingTime);
    }
}